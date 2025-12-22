using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class OrdersPage : ContentPage
    {
        public OrderViewModel ViewModel { get; }

        public OrdersPage(OrderViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.LoadOrdersCommand?.Execute(null);
        }
    }
}
