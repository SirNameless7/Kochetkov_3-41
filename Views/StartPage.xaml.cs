using KPO_Cursovoy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public partial class StartPage : ContentPage
{
    public StartPageViewModel ViewModel { get; }

    public StartPage()
        : this(App.ServiceProvider.GetRequiredService<StartPageViewModel>())
    {
    }

    public StartPage(StartPageViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        BindingContext = ViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
