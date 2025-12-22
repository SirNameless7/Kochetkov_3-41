using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class OrderDetailPage : ContentPage
    {
        public OrderDetailViewModel ViewModel { get; }

        public OrderDetailPage(OrderDetailViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = ViewModel?.InitializeAsync(1001);
        }
    }
}
