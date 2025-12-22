using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using System.Windows.Input;

namespace KPO_Cursovoy.ViewModels
{
    public class PcDetailViewModel : BaseViewModel
    {
        private readonly CartService _cartService;
        private readonly INavigationService _navigationService;

        private PcItem _pc;
        public PcItem Pc
        {
            get => _pc;
            set => SetProperty(ref _pc, value);
        }

        public ICommand AddToCartCommand { get; }
        public ICommand BackCommand { get; }

        public PcDetailViewModel(CartService cartService, INavigationService navigationService)
        {
            _cartService = cartService;
            _navigationService = navigationService;
            AddToCartCommand = new Command(OnAddToCart);
            BackCommand = new Command(OnBack);
        }

        public void Initialize(PcItem pc)
        {
            Pc = pc;
        }

        private void OnAddToCart()
        {
            if (Pc != null)
            {
                _cartService.AddItem(new CartItem { Pc = Pc, Quantity = 1 });
            }
        }

        private async void OnBack()
        {
            await _navigationService.GoBackAsync();
        }
    }
}
