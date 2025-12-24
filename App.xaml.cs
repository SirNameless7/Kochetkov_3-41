using System;
using KPO_Cursovoy.Models;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy;

public partial class App : Application
{
    public static User? CurrentUser { get; set; }

    private readonly IServiceProvider _serviceProvider;

    public static IServiceProvider ServiceProvider =>
        (Application.Current as App)?._serviceProvider
        ?? throw new InvalidOperationException("ServiceProvider is not initialized.");

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;

        // Shell сам откроет первый ShellItem -> StartPage (см. AppShell.xaml)
        MainPage = new AppShell();
    }

    public void ClearSession()
    {
        CurrentUser = null;
    }

    public void Logout()
    {
        ClearSession();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (Shell.Current != null)
                await Shell.Current.GoToAsync("//StartPage"); // теперь это Shell route (не global)
        });
    }

    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(MainPage);
}
