using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class CartPage : ContentPage
    {
        public CartViewModel ViewModel { get; }

        public CartPage(CartViewModel viewModel)
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
