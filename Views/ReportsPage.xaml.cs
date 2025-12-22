using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class ReportsPage : ContentPage
    {
        public ReportsViewModel ViewModel { get; }

        public ReportsPage(ReportsViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.Initialize();
        }
    }
}
