using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class ServicesPage : ContentPage
    {
        public ServicesViewModel ViewModel { get; }

        public ServicesPage(ServicesViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.LoadServicesCommand?.Execute(null);
        }
    }
}
