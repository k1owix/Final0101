using DesktopApp.Helpers;
using DesktopApp.Models;
using DesktopApp.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopApp
{
    public partial class MainWindow : Window
    {
        private readonly IApiService _api = new ApiService("https://localhost:7046");
        private List<Product> _products = new();

        public MainWindow()
        {
            InitializeComponent();
            LoadManufacturers();
            LoadProducts();
            UpdateUI();
            UpdateCart();
        }

        private async void LoadProducts()
        {
            try
            {
                var search = SearchTextBox?.Text ?? "";

                var manufacturer = ManufacturerComboBox?.SelectedItem?.ToString();
                if (manufacturer == "Все производители") manufacturer = null;

                var minPrice = ParsePrice(MinPriceTextBox?.Text ?? "0");
                var maxPrice = ParsePrice(MaxPriceTextBox?.Text ?? "100000");

                var sortBy = SortComboBox?.SelectedItem?.ToString();
                if (sortBy == "Имя") sortBy = "name";
                else if (sortBy == "Цена") sortBy = "price";

                _products = await _api.GetProductsAsync(
                    search: search,
                    manufacturer: manufacturer,
                    minPrice: minPrice,
                    maxPrice: maxPrice,
                    sortBy: sortBy,
                    desc: DescCheckBox?.IsChecked ?? false
                );

                if (ProductsPanel != null)
                {
                    ProductsPanel.Children.Clear();

                    if (_products.Count == 0)
                    {
                        ProductsPanel.Children.Add(new TextBlock
                        { Text = "Товары не найдены", FontSize = 16, Margin = new Thickness(20) });
                        return;
                    }

                    foreach (var p in _products)
                        ProductsPanel.Children.Add(CreateCard(p));
                }

                if (CountTextBlock != null)
                    CountTextBlock.Text = $"Найдено: {_products.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private decimal ParsePrice(string text) => decimal.TryParse(text, out var v) ? v : 0;

        private Border CreateCard(Product p)
        {
            var card = new Border
            {
                Width = 200,
                Height = 240,
                Margin = new Thickness(10),
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                Background = new SolidColorBrush(Colors.White)
            };

            var stack = new StackPanel();

            stack.Children.Add(new TextBlock
            {
                Text = p.Name,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                Height = 65
            });

            stack.Children.Add(new TextBlock
            {
                Text = $"Производитель: {p.Manufacturer}",
                FontSize = 11,
                Foreground = new SolidColorBrush(Colors.Gray)
            });

            var price = new TextBlock
            {
                Text = $"{p.Price:C}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(184, 134, 11))
            };

            if (p.Discount > 0)
            {
                var discounted = p.Price * (1 - p.Discount / 100m);
                price.Text = $"{discounted:C}";
                stack.Children.Add(new TextBlock
                {
                    Text = $"{p.Price:C} -{p.Discount}%",
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    TextDecorations = TextDecorations.Strikethrough
                });
            }
            stack.Children.Add(price);

            var btn = new Button
            {
                Content = "ЗАКАЗАТЬ",
                Background = new SolidColorBrush(Color.FromRgb(184, 134, 11)),
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.Bold,
                Height = 30,
                Margin = new Thickness(0, 10, 0, 0)
            };
            btn.Click += (s, e) => AddToCart(p);

            stack.Children.Add(btn);
            card.Child = stack;
            return card;
        }

        private void AddToCart(Product p)
        {
            var item = AppState.Cart.FirstOrDefault(c => c.Product.ArticleNumber == p.ArticleNumber);
            if (item != null) item.Quantity++;
            else AppState.Cart.Add(new CartItem { Product = p, Quantity = 1 });

            UpdateCart();
            MessageBox.Show($"{p.Name} добавлен в корзину!");
        }

        private void UpdateCart()
        {
            var total = AppState.Cart.Sum(c => c.Quantity);
            if (CartButton != null)
            {
                if (total > 0)
                {
                    CartButton.Visibility = Visibility.Visible;
                    CartButton.Content = $"Корзина ({total})";
                }
                else
                {
                    CartButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void LoadManufacturers()
        {
            try
            {
                var list = await _api.GetManufacturersAsync();
                if (ManufacturerComboBox != null)
                {
                    ManufacturerComboBox.Items.Clear();
                    ManufacturerComboBox.Items.Add("Все производители");
                    foreach (var m in list) ManufacturerComboBox.Items.Add(m);
                    ManufacturerComboBox.SelectedIndex = 0;
                }
            }
            catch { }
        }

        private void UpdateUI()
        {
            if (AppState.IsAuthenticated)
            {
                LoginButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Visible;
                UserLabel.Text = AppState.UserName;

                if (AppState.UserRole == "Администратор" || AppState.UserRole == "Менеджер")
                    ManageOrdersButton.Visibility = Visibility.Visible;
                else
                    ManageOrdersButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoginButton.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Collapsed;
                UserLabel.Text = "Гость";
                ManageOrdersButton.Visibility = Visibility.Collapsed;
            }
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e) => LoadProducts();

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            if (login.ShowDialog() == true)
            {
                UpdateUI();
                UpdateCart();
                LoadProducts();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            AppState.Clear();
            UpdateUI();
            UpdateCart();
            LoadProducts();
            MessageBox.Show("Вы вышли");
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.Cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }
            var orderWindow = new OrderWindow();
            if (orderWindow.ShowDialog() == true)
            {
                UpdateCart();
                LoadProducts();
            }
        }

        private void ManageOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var orders = new OrderManagementWindow();
            orders.ShowDialog();
        }
    }
}