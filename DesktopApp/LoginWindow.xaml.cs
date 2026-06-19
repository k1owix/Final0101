using DesktopApp.Helpers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace DesktopApp
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _http = new HttpClient();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await _http.PostAsJsonAsync(
                    "https://localhost:7046/api/users/login",
                    new { login = LoginTextBox.Text, password = PasswordBox.Password }
                );

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var user = await response.Content.ReadFromJsonAsync<LoginResponse>();
                AppState.UserName = user.FullName;
                AppState.UserRole = user.Role;
                AppState.UserId = user.UserId;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class LoginResponse
    {
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "";
        public int UserId { get; set; }
    }
}