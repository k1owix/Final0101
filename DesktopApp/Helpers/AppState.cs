using DesktopApp.Models;

namespace DesktopApp.Helpers
{
    public static class AppState
    {
        public static string? UserName { get; set; }
        public static string? UserRole { get; set; }
        public static int? UserId { get; set; }
        public static bool IsAuthenticated => !string.IsNullOrEmpty(UserName);
        public static bool IsAdminOrManager => UserRole == "Администратор" || UserRole == "Менеджер";
        public static List<CartItem> Cart { get; set; } = new();

        public static void Clear()
        {
            UserName = null;
            UserRole = null;
            UserId = null;
            Cart.Clear();
        }
    }
}