using KPO_Cursovoy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public partial class ServicesPage : ContentPage
{
    public ServicesViewModel ViewModel { get; }

    public ServicesPage()
        : this(App.ServiceProvider.GetRequiredService<ServicesViewModel>())
    {
    }

    public ServicesPage(ServicesViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        BindingContext = ViewModel;
    }
}
