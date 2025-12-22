using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class AdminPage : ContentPage
    {
        public AdminViewModel ViewModel { get; }

        public AdminPage(AdminViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.RefreshCommand?.Execute(null);
        }
    }
}
