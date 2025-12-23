using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class OrderDetailPage : ContentPage
    {
        private readonly int _orderId;

        public OrderDetailPage(OrderDetailViewModel vm, int orderId)
        {
            InitializeComponent();
            BindingContext = vm;
            _orderId = orderId;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_orderId > 0)
                await ((OrderDetailViewModel)BindingContext).InitializeAsync(_orderId);
        }
    }


}
