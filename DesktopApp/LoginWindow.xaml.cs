using DesktopApp.Helpers;
using DesktopApp.Services;
using System.Windows;

namespace DesktopApp;

public partial class LoginWindow : Window
{
    private readonly ApiService _apiService = new();

    public LoginWindow()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var login = LoginTextBox.Text.Trim();
        var password = PasswordBox.Password.Trim();

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Введите логин и пароль.", "Недостаточно данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var authenticatedUser = await ApiService.LoginAsync(login, password);
            AppState.SetCurrentUser(authenticatedUser);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Вход не выполнен.\n{ex.Message}", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
