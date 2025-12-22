using KPO_Cursovoy.ViewModels;
using KPO_Cursovoy.Models;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Views
{
    public partial class PcDetailPage : ContentPage
    {
        public PcDetailViewModel ViewModel { get; }

        public PcDetailPage(PcDetailViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            BindingContext = ViewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        public void Initialize(PcItem pc)
        {
            ViewModel.Initialize(pc);
        }
    }
}
