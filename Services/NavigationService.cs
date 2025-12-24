using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KPO_Cursovoy.Constants;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.ViewModels;
using KPO_Cursovoy.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task NavigateToAsync(string route) => NavigateToAsync(route, null);

    public async Task NavigateToAsync(string route, object? parameter)
    {
        if (Application.Current?.MainPage is Shell && Shell.Current != null)
        {
            // Все ShellContent-страницы (и auth, и tabbar) переключаем через //
            if (route is Routes.StartPage or Routes.LoginPage or Routes.RegisterPage
                or Routes.MainPage or Routes.BuildPcPage or Routes.CartPage or Routes.OrdersPage or Routes.ProfilePage)
            {
                await Shell.Current.GoToAsync($"//{route}");
                return;
            }

            // Остальное пушим в стек текущей секции
            var page = CreatePageByRoute(route, parameter);
            await Shell.Current.Navigation.PushAsync(page);
            return;
        }

        // Фолбэк
        var fallbackPage = CreatePageByRoute(route, parameter);

        if (Application.Current?.MainPage is NavigationPage nav)
            await nav.PushAsync(fallbackPage);
        else if (Application.Current != null)
            Application.Current.MainPage = new NavigationPage(fallbackPage);
    }

    public async Task GoBackAsync()
    {
        if (Application.Current?.MainPage is Shell && Shell.Current != null)
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
                await Shell.Current.Navigation.PopAsync();
            return;
        }

        if (Application.Current?.MainPage is NavigationPage nav)
            await nav.PopAsync();
    }

    private ContentPage CreatePageByRoute(string route, object? parameter)
    {
        return route switch
        {
            Routes.ServicesPage => CreatePage<ServicesPage, ServicesViewModel>(),
            Routes.AdminPage => CreatePage<AdminPage, AdminViewModel>(),
            Routes.ReportsPage => CreatePage<ReportsPage, ReportsViewModel>(),

            Routes.PcDetailPage => CreatePcDetailPage(parameter),
            Routes.OrderDetailPage => CreateOrderDetailPage(parameter),

            _ => new ContentPage { Title = $"Страница {route} не найдена" }
        };
    }

    private TPage CreatePage<TPage, TViewModel>()
        where TPage : ContentPage
        where TViewModel : class
    {
        var vm = _serviceProvider.GetRequiredService<TViewModel>();
        return (TPage)Activator.CreateInstance(typeof(TPage), vm)!;
    }

    private PcDetailPage CreatePcDetailPage(object? parameter)
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

    private OrderDetailPage CreateOrderDetailPage(object? parameter)
    {
        var vm = _serviceProvider.GetRequiredService<OrderDetailViewModel>();

        var orderId = 0;
        if (parameter is Dictionary<string, object> dict &&
            dict.TryGetValue("OrderId", out var idObj))
        {
            if (idObj is int id) orderId = id;
            else if (idObj is long longId) orderId = (int)longId;
            else if (idObj is string s && int.TryParse(s, out var parsed)) orderId = parsed;
        }

        return new OrderDetailPage(vm, orderId);
    }
}
