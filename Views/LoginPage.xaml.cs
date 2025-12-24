using KPO_Cursovoy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public partial class LoginPage : ContentPage
{
    public LoginViewModel ViewModel { get; }

    public LoginPage()
        : this(App.ServiceProvider.GetRequiredService<LoginViewModel>())
    {
    }

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        BindingContext = ViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel?.InitializeAsync();
    }
}
