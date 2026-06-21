using DesktopApp.Models;
using DesktopApp.Services;
using System.Collections.ObjectModel;

namespace DesktopApp.Helpers;

public static class AppState
{
    static AppState()
    {
        CurrentUser = UserSessionStore.Load();
    }

    public static AuthenticatedUser? CurrentUser { get; private set; }
    public static bool IsAuthenticated => CurrentUser is not null;
    public static bool CanManageOrders =>
        CurrentUser?.RoleName is "Администратор" or "Менеджер";
    public static ObservableCollection<CartItem> Cart { get; } = [];

    public static void SetCurrentUser(AuthenticatedUser user)
    {
        CurrentUser = user;
        UserSessionStore.Save(user);
    }

    public static void SignOut()
    {
        CurrentUser = null;
        Cart.Clear();
        UserSessionStore.Clear();
    }
}
