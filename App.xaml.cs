using System;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Views;
using KPO_Cursovoy.ViewModels;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.EntityFrameworkCore;

namespace KPO_Cursovoy
{
    public partial class App : Application
    {
        public static User? CurrentUser { get; set; }
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            LoadSavedSession();
        }

        private async void LoadSavedSession()
        {
            string savedPhone = Preferences.Get("SavedPhone", string.Empty);
            if (!string.IsNullOrEmpty(savedPhone))
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var authService = scope.ServiceProvider.GetRequiredService<AuthenticationService>();

                    var user = await authService.GetUserByPhoneAsync(savedPhone);

                    if (user != null)
                    {
                        CurrentUser = user;
                        MainPage = new AppShell();  
                    }
                    else
                    {
                        ClearSession();
                        MainPage = CreateStartPageNavigation();
                    }
                }
                catch
                {
                    ClearSession();
                    MainPage = CreateStartPageNavigation();
                }
            }
            else
            {
                MainPage = CreateStartPageNavigation();
            }
        }

        private Page CreateStartPageNavigation()
        {
            var startPageViewModel = _serviceProvider.GetRequiredService<StartPageViewModel>();
            var startPage = new StartPage(startPageViewModel);
            return new NavigationPage(startPage);
        }

        public void ClearSession()
        {
            CurrentUser = null;
            Preferences.Remove("SavedPhone");
        }

        public void Logout()
        {
            ClearSession();
            MainPage = CreateStartPageNavigation();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(MainPage);
        }
    }
}
