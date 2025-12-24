using KPO_Cursovoy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public partial class BuildPcPage : ContentPage
{
    public BuildPcViewModel ViewModel { get; }

    public BuildPcPage()
        : this(App.ServiceProvider.GetRequiredService<BuildPcViewModel>())
    {
    }

    public BuildPcPage(BuildPcViewModel viewModel)
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
