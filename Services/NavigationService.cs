using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using KPO_Cursovoy.Views;
using KPO_Cursovoy.ViewModels;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task NavigateToAsync(string route)
        {
            await Navigate(route, null);
        }

        public async Task NavigateToAsync(string route, object parameter)
        {
            await Navigate(route, parameter);
        }

        public async Task GoBackAsync()
        {
            if (Application.Current.MainPage is NavigationPage navPage)
            {
                await navPage.PopAsync();
            }
        }

        private async Task Navigate(string route, object parameter)
        {
            ContentPage page = route switch
            {
                "StartPage" => CreatePage<StartPage, StartPageViewModel>(),
                "LoginPage" => CreatePage<LoginPage, LoginViewModel>(),
                "RegisterPage" => CreatePage<RegisterPage, RegisterViewModel>(),
                "MainPage" => CreatePage<MainPage, MainPageViewModel>(),
                "BuildPcPage" => CreatePage<BuildPcPage, BuildPcViewModel>(),
                "CartPage" => CreatePage<CartPage, CartViewModel>(),
                "OrdersPage" => CreatePage<OrdersPage, OrderViewModel>(),
                "ServicesPage" => CreatePage<ServicesPage, ServicesViewModel>(),
                "PcDetailPage" => CreatePcDetailPage(parameter),
                "OrderDetailPage" => CreatePage<OrderDetailPage, OrderDetailViewModel>(),
                "ProfilePage" => CreatePage<ProfilePage, ProfileViewModel>(),
                "AdminPage" => CreatePage<AdminPage, AdminViewModel>(),
                "ReportsPage" => CreatePage<ReportsPage, ReportsViewModel>(),
                _ => new ContentPage { Title = $"Страница {route} не найдена" }
            };

            if (Application.Current.MainPage is NavigationPage navPage)
                await navPage.PushAsync(page);
            else
                Application.Current.MainPage = new NavigationPage(page);
        }

        private TPage CreatePage<TPage, TViewModel>()
            where TPage : ContentPage
            where TViewModel : class
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            var page = (TPage)Activator.CreateInstance(typeof(TPage), viewModel)!;
            return page;
        }

        private PcDetailPage CreatePcDetailPage(object parameter)
        {
            var vm = _serviceProvider.GetRequiredService<PcDetailViewModel>();
            var page = new PcDetailPage(vm);

            if (parameter is Dictionary<string, object> dict &&
                dict.TryGetValue("Pc", out var pcObj) &&
                pcObj is PcItem pc)
            {
                page.Initialize(pc);
            }

            return page;
        }
    }
}
