using DesktopApp.Helpers;
using DesktopApp.Models;
using DesktopApp.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp;

public partial class OrderManagementWindow : Window
{
    private readonly ApiService _apiService = new();
    private IReadOnlyList<OrderSummary> _orders = [];

    public OrderManagementWindow()
    {
        InitializeComponent();
        Loaded += OrderManagementWindow_Loaded;
    }

    private async void OrderManagementWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (!AppState.CanManageOrders)
        {
            MessageBox.Show(
                "Окно управления заказами доступно только менеджеру или администратору.",
                "Доступ запрещен",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            Close();
            return;
        }

        await LoadOrdersAsync();
    }

    private async Task LoadOrdersAsync()
    {
        try
        {
            _orders = await ApiService.GetOrdersAsync();
            OrdersDataGrid.ItemsSource = _orders;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось загрузить заказы.\n{ex.Message}", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (OrdersDataGrid.SelectedItem is not OrderSummary order)
        {
            return;
        }

        DeliveryDatePicker.SelectedDate = order.DeliveryDate;
        SelectStatus(order.StatusName);
        OrderDetailsTextBlock.Text = BuildOrderDetails(order);
    }

    private async void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        if (OrdersDataGrid.SelectedItem is not OrderSummary order)
        {
            MessageBox.Show("Сначала выберите заказ из списка.", "Нет выбранного заказа", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (StatusComboBox.SelectedItem is not ComboBoxItem selectedStatus)
        {
            MessageBox.Show("Укажите новый статус заказа.", "Нет статуса", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            await _apiService.UpdateOrderAsync(
                order.OrderId,
                new OrderUpdateRequest
                {
                    DeliveryDate = DeliveryDatePicker.SelectedDate,
                    StatusName = selectedStatus.Content?.ToString() ?? string.Empty
                });

            await LoadOrdersAsync();
            MessageBox.Show("Изменения по заказу сохранены.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось обновить заказ.\n{ex.Message}", "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SelectStatus(string statusName)
    {
        foreach (ComboBoxItem item in StatusComboBox.Items)
        {
            if (string.Equals(item.Content?.ToString(), statusName, StringComparison.OrdinalIgnoreCase))
            {
                StatusComboBox.SelectedItem = item;
                return;
            }
        }
    }

    private static string BuildOrderDetails(OrderSummary order)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Клиент: {order.ClientName ?? "Гость"}");
        builder.AppendLine($"Код получения: {order.ReceiptCode}");
        builder.AppendLine("Состав заказа:");

        foreach (var item in order.Items)
        {
            builder.AppendLine($"- {item.ProductName} ({item.Quantity} шт.)");
        }

        return builder.ToString().TrimEnd();
    }
}
