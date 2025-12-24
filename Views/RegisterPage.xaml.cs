using KPO_Cursovoy.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterViewModel ViewModel { get; }

    public RegisterPage()
        : this(App.ServiceProvider.GetRequiredService<RegisterViewModel>())
    {
    }

    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        BindingContext = ViewModel;
    }
}
