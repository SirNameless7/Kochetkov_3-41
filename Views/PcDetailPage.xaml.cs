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
            var samplePc = new PcItem
            {
                Id = 1,
                Name = "Gaming PC RTX 4080",
                Description = "Мощный игровой ПК с RTX 4080 и i7-13700K",
                Price = 185000,
                //ImageUrl = "gamingpc.png"
            };

            ViewModel.Initialize(samplePc);
        }
    }
}
