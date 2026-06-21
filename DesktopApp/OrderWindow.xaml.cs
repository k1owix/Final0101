using DesktopApp.Helpers;
using DesktopApp.Models;
using DesktopApp.Services;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp;

public partial class OrderWindow : Window
{
    private readonly ApiService _apiService = new();

    public OrderWindow()
    {
        InitializeComponent();
        LoadCart();
    }

    private void LoadCart()
    {
        CustomerTextBlock.Text = $"Покупатель: {AppState.CurrentUser?.FullName ?? "гость"}";
        ItemsListView.ItemsSource = AppState.Cart.ToList();
        TotalTextBlock.Text = AppState.Cart.Sum(item => item.Total).ToString("C");
    }

    private async void OrderButton_Click(object sender, RoutedEventArgs e)
    {
        if (AppState.Cart.Count == 0)
        {
            MessageBox.Show("Добавьте в корзину хотя бы один товар.", "Пустая корзина", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var createdOrder = await ApiService.CreateOrderAsync(
                new OrderCreateRequest
                {
                    UserId = AppState.CurrentUser?.UserId,
                    Items = [.. AppState.Cart
                        .Select(item => new OrderCreateItemRequest
                        {
                            ProductId = item.Product.ProductId,
                            Quantity = item.Quantity
                        })]
                });

            var receiptText = BuildReceipt(createdOrder);
            SaveReceipt(createdOrder.OrderNumber, receiptText);
            AppState.Cart.Clear();

            MessageBox.Show(
                $"Заказ №{createdOrder.OrderNumber} оформлен.\nКод получения: {createdOrder.ReceiptCode}",
                "Заказ оформлен",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось оформить заказ.\n{ex.Message}", "Ошибка оформления", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static string BuildReceipt(CreatedOrder createdOrder)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Читайте город");
        builder.AppendLine($"Номер заказа: {createdOrder.OrderNumber}");
        builder.AppendLine($"Дата заказа: {createdOrder.OrderDate:dd.MM.yyyy HH:mm}");
        builder.AppendLine($"Покупатель: {AppState.CurrentUser?.FullName ?? "Гость"}");
        builder.AppendLine($"Код получения: {createdOrder.ReceiptCode}");
        builder.AppendLine(new string('-', 42));

        foreach (var item in AppState.Cart)
        {
            builder.AppendLine(item.Product.Name);
            builder.AppendLine($"  {item.Quantity} x {item.Product.DiscountedPrice:C} = {item.Total:C}");
        }

        builder.AppendLine(new string('-', 42));
        builder.AppendLine($"Итого: {AppState.Cart.Sum(item => item.Total):C}");
        return builder.ToString();
    }

    private void SaveReceipt(int orderNumber, string receiptText)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Сохранить талон",
            FileName = $"Заказ_{orderNumber}.txt",
            Filter = "Текстовые файлы (*.txt)|*.txt",
            DefaultExt = ".txt"
        };

        if (dialog.ShowDialog(this) == true)
        {
            File.WriteAllText(dialog.FileName, receiptText, Encoding.UTF8);
        }
    }

    private void RemoveButton_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is not CartItem cartItem)
        {
            return;
        }

        AppState.Cart.Remove(cartItem);
        LoadCart();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
