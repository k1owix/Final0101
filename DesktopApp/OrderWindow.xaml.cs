using DataAccess.Entities;
using DataAccess.Models;
using DesktopApp.Helpers;
using DesktopApp.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp
{
    public partial class OrderWindow : Window
    {
        private readonly HttpClient _http = new();

        public OrderWindow()
        {
            InitializeComponent();
            LoadCart();
        }

        private void LoadCart()
        {
            ItemsListView.ItemsSource = AppState.Cart.ToList();
            var total = AppState.Cart.Sum(c => c.Total);
            TotalTextBlock.Text = total.ToString("C");
        }

        private async void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.Cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста!");
                return;
            }

            try
            {
                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerName = AppState.UserName,
                    Status = "Новый",
                    OrderProducts = [.. AppState.Cart.Select(c => new OrderProduct
                    {
                        ProductId = c.Product.ArticleNumber,
                        Quantity = c.Quantity
                    })]
                };

                var response = await _http.PostAsJsonAsync("https://localhost:7046/api/orders", order);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка: {error}");
                    return;
                }

                var created = await response.Content.ReadFromJsonAsync<Order>();
                MessageBox.Show($"Заказ №{created.OrderId} оформлен!\nКод получения: {created.PickupCode}",
                    "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                AppState.Cart.Clear();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            CartItem? item = (sender as Button)?.Tag as CartItem;
            if (item != null && AppState.Cart.Remove(item)) LoadCart();
        }
    }
}