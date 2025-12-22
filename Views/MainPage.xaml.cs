using KPO_Cursovoy.ViewModels;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel ViewModel { get; }

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.InitializeAsync();
        }
    }
}
