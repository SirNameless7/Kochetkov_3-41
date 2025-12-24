using System.Linq;
using Microsoft.Maui.Controls;
using KPO_Cursovoy.Constants;

namespace KPO_Cursovoy;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        RegisterAllRoutes();
        Navigating += OnNavigating;
    }

    private void RegisterAllRoutes()
    {
        // Эти страницы не надо регистрировать, они уже в AppShell.xaml:
        // StartPage, LoginPage, RegisterPage, MainPage, BuildPcPage, CartPage, OrdersPage, ProfilePage

        // Доп. страницы (если где-то будешь переходить по роуту)
        Routing.RegisterRoute(Routes.ServicesPage, typeof(Views.ServicesPage));
        Routing.RegisterRoute(Routes.AdminPage, typeof(Views.AdminPage));
        Routing.RegisterRoute(Routes.ReportsPage, typeof(Views.ReportsPage));
        Routing.RegisterRoute(Routes.PcDetailPage, typeof(Views.PcDetailPage));
        Routing.RegisterRoute(Routes.OrderDetailPage, typeof(Views.OrderDetailPage));
    }

    private async void OnNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        var target = (e.Target?.Location.OriginalString ?? string.Empty).ToLowerInvariant();

        // Всегда разрешаем auth-страницы
        if (target.Contains("startpage") || target.Contains("loginpage") || target.Contains("registerpage"))
            return;

        // Каталог доступен гостю
        if (target.Contains(Routes.MainPage.ToLowerInvariant()))
            return;

        var protectedRoutes = new[]
        {
            Routes.BuildPcPage,
            Routes.CartPage,
            Routes.OrdersPage,
            Routes.ProfilePage,
            Routes.ServicesPage,
            Routes.AdminPage,
            Routes.ReportsPage
        }
        .Select(r => r.ToLowerInvariant())
        .ToArray();

        var isProtected = protectedRoutes.Any(r => target.Contains(r));
        if (!isProtected)
            return;

        if (App.CurrentUser != null)
            return;

        e.Cancel();
        await Shell.Current.GoToAsync("//LoginPage");
        await Shell.Current.DisplayAlert("Требуется авторизация", "Пожалуйста, войдите в систему", "ОК");
    }
}
