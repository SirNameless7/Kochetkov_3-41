using KPO_Cursovoy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public partial class ProfilePage : ContentPage
{
    public ProfileViewModel ViewModel { get; }

    public ProfilePage()
        : this(App.ServiceProvider.GetRequiredService<ProfileViewModel>())
    {
    }

    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        BindingContext = ViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.InitializeAsync();
    }
}
