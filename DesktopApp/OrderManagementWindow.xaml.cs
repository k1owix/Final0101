using DataAccess.Entities;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp
{
    public partial class OrderManagementWindow : Window
    {
        private readonly HttpClient _http = new();
        private List<Order> _orders = [];

        public OrderManagementWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private async void LoadOrders()
        {
            var orders = await _http.GetFromJsonAsync<List<Order>>("https://localhost:7046/api/orders");
            _orders = orders ?? [];
            OrdersListView.ItemsSource = _orders;
        }

        private void OrdersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersListView.SelectedItem is Order order)
            {
                StatusComboBox.Text = order.Status;
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersListView.SelectedItem is Order order)
            {
                order.Status = StatusComboBox.Text;
                await _http.PutAsJsonAsync($"https://localhost:7046/api/orders/{order.OrderId}", order);
                MessageBox.Show("Статус обновлён!");
                LoadOrders();
            }
        }
    }
}
