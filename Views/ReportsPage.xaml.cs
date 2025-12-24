using KPO_Cursovoy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public partial class ReportsPage : ContentPage
{
    public ReportsViewModel ViewModel { get; }

    public ReportsPage()
        : this(App.ServiceProvider.GetRequiredService<ReportsViewModel>())
    {
    }

    public ReportsPage(ReportsViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        BindingContext = ViewModel;
    }
}
