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
        public string CurrentSection => "Cart";

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
        public ICommand NavigateToBuildPcCommand { get; }
        public ICommand NavigateToCartCommand { get; }
        public ICommand NavigateToOrdersCommand { get; }
        public ICommand NavigateToProfileCommand { get; }

        public CartViewModel(CartService cartService,
                             INavigationService navigationService,
                             DatabaseService databaseService)
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

            NavigateToBuildPcCommand = new Command(async () =>
                await _navigationService.NavigateToAsync(Routes.BuildPcPage));

            NavigateToCartCommand = new Command(async () =>
                await _navigationService.NavigateToAsync(Routes.CartPage));

            NavigateToOrdersCommand = new Command(async () =>
                await _navigationService.NavigateToAsync(Routes.OrdersPage));

            NavigateToProfileCommand = new Command(async () =>
                await _navigationService.NavigateToAsync(Routes.ProfilePage));

            LoadCart();
            _cartService.CartChanged += (s, e) => LoadCart();
        }

        private void LoadCart()
        {
            Items.Clear();
            foreach (var item in _cartService.Items)
                Items.Add(item);

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

            item.Quantity++;
            RefreshItem(item);
            UpdateTotal();
        }

        private void OnDecrease(CartItem item)
        {
            if (item == null) return;

            if (item.Quantity > 1)
            {
                item.Quantity--;
                RefreshItem(item);
            }
            else
            {
                _cartService.RemoveItem(item);
                Items.Remove(item);
            }

            UpdateTotal();
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
                return;

            try
            {
                IsBusy = true;
                var usedComponents = new List<ComponentItem>();
                foreach (var item in Items)
                {
                    if (item.Component != null)
                    {
                        for (int i = 0; i < item.Quantity; i++)
                            usedComponents.Add(item.Component);
                    }
                    else if (item.Pc != null && item.IsCustomBuild && item.Pc.Components != null)
                    {
                        foreach (var comp in item.Pc.Components)
                            usedComponents.Add(comp);
                    }
                }

                foreach (var comp in usedComponents)
                {
                    if (comp.Stock < 1)
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Ошибка",
                            $"Компонент {comp.Name} закончился на складе",
                            "ОК");
                        return;
                    }
                }
                var order = await CreateOrderAsync(usedComponents);
                if (order != null)
                {
                    _cartService.Clear();
                    Items.Clear();
                    TotalPrice = 0;

                    await _navigationService.NavigateToAsync(Routes.OrdersPage);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<Order?> CreateOrderAsync(List<ComponentItem> usedComponents)
        {
            var currentUser = App.CurrentUser;
            if (currentUser == null)
                return null;

            var order = new Order
            {
                UserId = currentUser.UserId,
                TotalAmount = TotalPrice,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.WaitingPayment
            };

            var componentQuantities = new Dictionary<int, int>();

            void AddComponent(int componentId, int qty)
            {
                if (componentQuantities.TryGetValue(componentId, out var existing))
                    componentQuantities[componentId] = existing + qty;
                else
                    componentQuantities[componentId] = qty;
            }

            foreach (var item in Items)
            {
                if (item.Component != null)
                {
                    AddComponent(item.Component.Id, item.Quantity);
                }
                else if (item.Pc != null)
                {
                    if (item.IsCustomBuild && item.Pc.Components != null)
                    {
                        foreach (var comp in item.Pc.Components)
                            AddComponent(comp.Id, 1);

                        order.IsCustomBuild = true;
                    }
                    else
                    {
                        order.PcId = item.Pc.Id;
                    }
                }
            }
            foreach (var kv in componentQuantities)
            {
                order.Components.Add(new OrderComponent
                {
                    ComponentId = kv.Key,
                    Quantity = kv.Value
                });
            }

            try
            {
                order = await _databaseService.CreateOrderAsync(order, usedComponents);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "ОК");
                return null;
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
            finally
            {
                IsBusy = false;
            }
        }

        private void RefreshItem(CartItem item)
        {
            var idx = Items.IndexOf(item);
            if (idx >= 0)
                Items[idx] = item; // триггерит CollectionChanged(Replace)
        }

    }
}
