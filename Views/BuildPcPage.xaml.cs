using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public partial class BuildPcPage : ContentPage
{
    public BuildPcViewModel ViewModel { get; }

    public BuildPcPage(BuildPcViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        ViewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel?.InitializeAsync();
    }

}
