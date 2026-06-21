using DesktopApp.Helpers;
using DesktopApp.Models;
using DesktopApp.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopApp;

public partial class MainWindow : Window
{
    private const string AllManufacturers = "Все издательства";

    private readonly ApiService _apiService = new();
    private IReadOnlyList<Product> _allProducts = [];
    private readonly bool _uiReady;

    public MainWindow()
    {
        InitializeComponent();
        _uiReady = true;
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadProductsAsync();
        PopulateManufacturers();
        UpdateUserState();
        ApplyFilters();
        UpdateCartButton();
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            _allProducts = await ApiService.GetProductsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не удалось загрузить товары.\n{ex.Message}",
                "Ошибка загрузки",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void PopulateManufacturers()
    {
        if (ManufacturerComboBox is null || MinPriceTextBox is null || MaxPriceTextBox is null)
        {
            return;
        }

        ManufacturerComboBox.Items.Clear();
        ManufacturerComboBox.Items.Add(AllManufacturers);

        foreach (var manufacturer in _allProducts
                     .Select(product => product.ManufacturerName)
                     .Distinct()
                     .OrderBy(name => name))
        {
            ManufacturerComboBox.Items.Add(manufacturer);
        }

        ManufacturerComboBox.SelectedIndex = 0;
        MinPriceTextBox.Text = "0";
        MaxPriceTextBox.Text = _allProducts.Count == 0
            ? "0"
            : decimal.Ceiling(_allProducts.Max(product => product.Price)).ToString("0");
    }

    private void UpdateUserState()
    {
        if (UserLabel is null || LoginButton is null || LogoutButton is null || ManageOrdersButton is null)
        {
            return;
        }

        UserLabel.Text = AppState.CurrentUser?.FullName ?? "Гость";
        LoginButton.Visibility = AppState.IsAuthenticated ? Visibility.Collapsed : Visibility.Visible;
        LogoutButton.Visibility = AppState.IsAuthenticated ? Visibility.Visible : Visibility.Collapsed;
        ManageOrdersButton.Visibility = AppState.CanManageOrders ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateCartButton()
    {
        if (CartButton is null)
        {
            return;
        }

        var totalQuantity = AppState.Cart.Sum(item => item.Quantity);
        CartButton.Visibility = totalQuantity > 0 ? Visibility.Visible : Visibility.Collapsed;
        CartButton.Content = totalQuantity > 0 ? $"Корзина ({totalQuantity})" : "Корзина";
    }

    private void ApplyFilters()
    {
        if (!_uiReady ||
            SearchTextBox is null ||
            ManufacturerComboBox is null ||
            MinPriceTextBox is null ||
            MaxPriceTextBox is null ||
            SortComboBox is null ||
            DescCheckBox is null ||
            CountTextBlock is null ||
            ProductsPanel is null)
        {
            return;
        }

        IEnumerable<Product> query = _allProducts;

        var search = SearchTextBox.Text.Trim();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(product => product.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase));
        }

        if (ManufacturerComboBox.SelectedItem is string manufacturer &&
            !string.Equals(manufacturer, AllManufacturers, StringComparison.Ordinal))
        {
            query = query.Where(product =>
                string.Equals(product.ManufacturerName, manufacturer, StringComparison.OrdinalIgnoreCase));
        }

        if (decimal.TryParse(MinPriceTextBox.Text, out var minPrice))
        {
            query = query.Where(product => product.DiscountedPrice >= minPrice);
        }

        if (decimal.TryParse(MaxPriceTextBox.Text, out var maxPrice))
        {
            query = query.Where(product => product.DiscountedPrice <= maxPrice);
        }

        var sortByPrice = (SortComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Цена";
        var isDescending = DescCheckBox.IsChecked ?? false;
        query = sortByPrice
            ? isDescending
                ? query.OrderByDescending(product => product.DiscountedPrice).ThenBy(product => product.Name)
                : query.OrderBy(product => product.DiscountedPrice).ThenBy(product => product.Name)
            : isDescending
                ? query.OrderByDescending(product => product.Name)
                : query.OrderBy(product => product.Name);

        var filteredProducts = query.ToList();
        CountTextBlock.Text = $"Показано: {filteredProducts.Count} из {_allProducts.Count}";

        ProductsPanel.Children.Clear();
        if (filteredProducts.Count == 0)
        {
            ProductsPanel.Children.Add(new TextBlock
            {
                Text = "По выбранным параметрам товары не найдены.",
                FontSize = 16,
                Margin = new Thickness(8)
            });
            return;
        }

        foreach (var product in filteredProducts)
        {
            ProductsPanel.Children.Add(CreateProductCard(product));
        }
    }

    private UIElement CreateProductCard(Product product)
    {
        var card = new Border
        {
            Width = 270,
            Margin = new Thickness(0, 0, 16, 16),
            Padding = new Thickness(14),
            CornerRadius = new CornerRadius(8),
            BorderBrush = new SolidColorBrush(Color.FromRgb(229, 224, 214)),
            BorderThickness = new Thickness(1),
            Background = Brushes.White
        };

        var layout = new Grid();
        for (var i = 0; i < 6; i++)
        {
            layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        var cover = new Image
        {
            Height = 180,
            Stretch = Stretch.Uniform,
            Margin = new Thickness(0, 0, 0, 12),
            Source = TryCreateImage(product.PhotoFileName)
        };
        Grid.SetRow(cover, 0);
        layout.Children.Add(cover);

        var title = new TextBlock
        {
            Text = product.Name,
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            TextWrapping = TextWrapping.Wrap
        };
        Grid.SetRow(title, 1);
        layout.Children.Add(title);

        var description = new TextBlock
        {
            Margin = new Thickness(0, 8, 0, 0),
            Text = string.IsNullOrWhiteSpace(product.Description) ? "Описание не указано." : product.Description,
            Foreground = new SolidColorBrush(Color.FromRgb(88, 88, 88)),
            TextWrapping = TextWrapping.Wrap,
            MaxHeight = 72
        };
        Grid.SetRow(description, 2);
        layout.Children.Add(description);

        var details = new StackPanel { Margin = new Thickness(0, 8, 0, 0) };
        details.Children.Add(new TextBlock
        {
            Text = $"Издательство: {product.ManufacturerName}",
            FontWeight = FontWeights.SemiBold
        });
        details.Children.Add(new TextBlock
        {
            Margin = new Thickness(0, 4, 0, 0),
            Text = product.StockQuantity > 0
                ? $"В наличии: {product.StockQuantity}"
                : "Нет в наличии, доступен предзаказ",
            Foreground = product.StockQuantity > 0 ? Brushes.DimGray : Brushes.IndianRed
        });
        Grid.SetRow(details, 3);
        layout.Children.Add(details);

        var pricePanel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };
        if (product.HasDiscount)
        {
            pricePanel.Children.Add(new TextBlock
            {
                Text = $"{product.Price:C}  -{product.Discount}%",
                Foreground = Brushes.Gray,
                TextDecorations = TextDecorations.Strikethrough
            });
        }

        pricePanel.Children.Add(new TextBlock
        {
            Text = $"{product.DiscountedPrice:C}",
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            Foreground = (Brush)FindResource("AccentBrush")
        });
        Grid.SetRow(pricePanel, 4);
        layout.Children.Add(pricePanel);

        var isPreorder = product.StockQuantity <= 0;
        var orderButton = new Button
        {
            Margin = new Thickness(0, 14, 0, 0),
            Content = isPreorder ? "Предзаказ" : "Заказать",
            Style = (Style)FindResource("AccentButtonStyle"),
            ToolTip = isPreorder
                ? "Книга временно закончилась, но ее можно добавить в заказ."
                : null
        };
        orderButton.Click += (_, _) => AddToCart(product);
        Grid.SetRow(orderButton, 5);
        layout.Children.Add(orderButton);

        card.Child = layout;
        return card;
    }

    private static ImageSource? TryCreateImage(string? photoFileName)
    {
        if (string.IsNullOrWhiteSpace(photoFileName))
        {
            return null;
        }

        try
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Resources/Products/{photoFileName}", UriKind.Absolute));
        }
        catch
        {
            return null;
        }
    }

    private void AddToCart(Product product)
    {
        var currentQuantity = AppState.Cart
            .Where(item => item.Product.ProductId == product.ProductId)
            .Sum(item => item.Quantity);

        if (product.StockQuantity > 0 && currentQuantity >= product.StockQuantity)
        {
            MessageBox.Show(
                "Для этого товара больше нет доступных экземпляров.",
                "Недостаточно товара",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        var existingItem = AppState.Cart.FirstOrDefault(item => item.Product.ProductId == product.ProductId);
        if (existingItem is null)
        {
            AppState.Cart.Add(new CartItem { Product = product, Quantity = 1 });
        }
        else
        {
            existingItem.Quantity += 1;
        }

        UpdateCartButton();
        MessageBox.Show(
            $"Товар \"{product.Name}\" добавлен в заказ.",
            "Товар добавлен",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void OnFilterChanged(object sender, RoutedEventArgs e)
    {
        if (!_uiReady)
        {
            return;
        }

        ApplyFilters();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var loginWindow = new LoginWindow { Owner = this };
        if (loginWindow.ShowDialog() == true)
        {
            UpdateUserState();
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        AppState.SignOut();
        UpdateUserState();
        UpdateCartButton();
        MessageBox.Show("Вы вышли из учетной записи.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void CartButton_Click(object sender, RoutedEventArgs e)
    {
        if (AppState.Cart.Count == 0)
        {
            MessageBox.Show("Корзина пока пуста.", "Нет товаров", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var orderWindow = new OrderWindow { Owner = this };
        if (orderWindow.ShowDialog() == true)
        {
            UpdateCartButton();
        }
    }

    private void ManageOrdersButton_Click(object sender, RoutedEventArgs e)
    {
        new OrderManagementWindow { Owner = this }.ShowDialog();
    }
}
