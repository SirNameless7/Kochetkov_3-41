using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterViewModel ViewModel { get; }

        public RegisterPage(RegisterViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }
    }
}