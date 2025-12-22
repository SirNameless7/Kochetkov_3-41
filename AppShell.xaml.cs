using System;
using System.Linq;
using KPO_Cursovoy.Models;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy
{
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
            Routing.RegisterRoute("StartPage", typeof(Views.StartPage));
            Routing.RegisterRoute("LoginPage", typeof(Views.LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(Views.RegisterPage));
            Routing.RegisterRoute("MainPage", typeof(Views.MainPage));
            Routing.RegisterRoute("BuildPcPage", typeof(Views.BuildPcPage));
            Routing.RegisterRoute("CartPage", typeof(Views.CartPage));
            Routing.RegisterRoute("OrdersPage", typeof(Views.OrdersPage));
            Routing.RegisterRoute("ServicesPage", typeof(Views.ServicesPage));
            Routing.RegisterRoute("PcDetailPage", typeof(Views.PcDetailPage));
            Routing.RegisterRoute("OrderDetailPage", typeof(Views.OrderDetailPage));
            Routing.RegisterRoute("ProfilePage", typeof(Views.ProfilePage));
            Routing.RegisterRoute("AdminPage", typeof(Views.AdminPage));
            Routing.RegisterRoute("ReportsPage", typeof(Views.ReportsPage));
        }

        private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {
            var protectedRoutes = new[]
            {
                "mainpage", "buildpcpage", "cartpage", "orderspage",
                "servicespage", "profilepage", "adminpage", "reportspage"
            };

            var currentRoute = e.Target.Location.OriginalString.ToLower();
            if (protectedRoutes.Any(route => currentRoute.Contains(route)))
            {
                if (App.CurrentUser == null)
                {
                    e.Cancel();
                    await Shell.Current.GoToAsync($"//StartPage");
                    await Shell.Current.DisplayAlert("Требуется авторизация",
                        "Пожалуйста, войдите в систему для доступа к этой странице", "ОК");
                    return;
                }

                if ((currentRoute.Contains("adminpage") || currentRoute.Contains("reportspage")) &&
                    App.CurrentUser.Account?.Role != "admin")
                {
                    e.Cancel();
                    await Shell.Current.DisplayAlert("Доступ запрещен",
                        "У вас нет прав для доступа к этой странице", "ОК");
                }
            }
        }

        public void UpdateAdminVisibility(bool isAdmin)
        {
            try
            {
                if (CurrentItem?.Items != null)
                {
                    var adminItem = CurrentItem.Items.FirstOrDefault(i => i.Route == "AdminPage");
                    if (adminItem != null)
                    {
                        adminItem.IsVisible = isAdmin;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateAdminVisibility error: {ex.Message}");
            }
        }
    }
}
