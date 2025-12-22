using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Constants;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        private readonly CartService _cartService;
        private readonly INavigationService _navigationService;
        private readonly DatabaseService _databaseService;

        public ObservableCollection<CartItem> Items { get; } = new();
        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set => SetProperty(ref _totalPrice, value);
        }
        public int TotalItemCount => Items.Sum(i => i.Quantity);
        public bool HasItems => Items.Count > 0;

        public ICommand IncreaseCommand { get; }
        public ICommand DecreaseCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand ClearCartCommand { get; }
        public ICommand NavigateToCatalogCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand IncreaseQuantityCommand => IncreaseCommand;
        public ICommand DecreaseQuantityCommand => DecreaseCommand;
        public ICommand RemoveItemCommand => RemoveCommand;

        public CartViewModel(CartService cartService, INavigationService navigationService, DatabaseService databaseService)
        {
            _cartService = cartService;
            _navigationService = navigationService;
            _databaseService = databaseService;

            IncreaseCommand = new Command<CartItem>(OnIncrease);
            DecreaseCommand = new Command<CartItem>(OnDecrease);
            RemoveCommand = new Command<CartItem>(OnRemove);
            CheckoutCommand = new AsyncCommand(OnCheckout);
            ClearCartCommand = new Command(OnClearCart);
            NavigateToCatalogCommand = new Command(OnNavigateToCatalog);
            RefreshCommand = new AsyncCommand(RefreshAsync);

            LoadCart();
            _cartService.CartChanged += (s, e) => LoadCart();
        }

        private void LoadCart()
        {
            Items.Clear();
            foreach (var item in _cartService.Items)
            {
                Items.Add(item);
            }
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            TotalPrice = Items.Sum(i => i.TotalPrice);
            OnPropertyChanged(nameof(TotalItemCount));
            OnPropertyChanged(nameof(HasItems));
        }

        private void OnIncrease(CartItem item)
        {
            if (item == null) return;

            var cartItem = _cartService.Items.FirstOrDefault(i => i.Id == item.Id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                UpdateTotal();
            }
        }

        private void OnDecrease(CartItem item)
        {
            if (item == null) return;

            var cartItem = _cartService.Items.FirstOrDefault(i => i.Id == item.Id);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                }
                else
                {
                    _cartService.RemoveItem(cartItem);
                    Items.Remove(item);
                }
                UpdateTotal();
            }
        }

        private void OnRemove(CartItem item)
        {
            if (item == null) return;

            _cartService.RemoveItem(item);
            Items.Remove(item);
            UpdateTotal();
        }

        private async Task OnCheckout()
        {
            if (Items.Count == 0)
            {
                return;
            }

            try
            {
                IsBusy = true;

                var isAvailable = await CheckAvailabilityAsync();
                if (!isAvailable)
                {
                    return;
                }

                var order = await CreateOrderAsync();
                if (order != null)
                {
                    _cartService.Clear();
                    Items.Clear();
                    TotalPrice = 0;

                    await _navigationService.NavigateToAsync(Routes.OrdersPage);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<bool> CheckAvailabilityAsync()
        {
            return true;
        }

        private async Task<Order?> CreateOrderAsync()
        {
            var currentUser = App.CurrentUser;
            if (currentUser == null)
            {
                return null;
            }
            var order = new Order
            {
                UserId = currentUser.UserId,
                TotalAmount = TotalPrice,
                OrderDate = DateTime.Now,
                Status = OrderStatus.WaitingPayment
            };

            foreach (var item in Items)
            {
                if (item.Component != null)
                {
                    order.Components.Add(new OrderComponent
                    {
                        ComponentId = item.Component.Id,
                        Quantity = item.Quantity
                    });
                }
                else if (item.Pc != null)
                {
                    order.PcId = item.Pc.Id;
                    order.IsCustomBuild = item.IsCustomBuild;
                }
            }
            try
            {
                order = await _databaseService.CreateOrderAsync(order);
            }
            catch
            {
                order.Id = new Random().Next(1000, 9999);
            }
            return order;
        }

        private void OnClearCart()
        {
            _cartService.Clear();
            Items.Clear();
            TotalPrice = 0;
        }

        private async void OnNavigateToCatalog()
        {
            await _navigationService.NavigateToAsync(Routes.MainPage);
        }

        private async Task RefreshAsync()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(500);
                LoadCart();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
