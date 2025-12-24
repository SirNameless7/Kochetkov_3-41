using KPO_Cursovoy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public partial class AdminPage : ContentPage
{
    public AdminViewModel ViewModel { get; }

    public AdminPage()
        : this(App.ServiceProvider.GetRequiredService<AdminViewModel>())
    {
    }

    public AdminPage(AdminViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        BindingContext = ViewModel;
    }
}
