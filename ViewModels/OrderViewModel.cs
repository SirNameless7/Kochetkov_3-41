using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Constants;

namespace KPO_Cursovoy.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        public string CurrentSection => "Orders";

        public ObservableCollection<Order> Orders { get; } = new();

        public ObservableCollection<string> StatusFilterOptions { get; } = new();
        private string? _selectedStatusFilter;
        public string? SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (SetProperty(ref _selectedStatusFilter, value))
                    _ = LoadOrdersAsync();
            }
        }

        public ICommand LoadOrdersCommand { get; }
        public ICommand ViewOrderDetailsCommand { get; }
        public ICommand NavigateToCatalogCommand { get; }
        public ICommand NavigateToBuildPcCommand { get; }
        public ICommand NavigateToCartCommand { get; }
        public ICommand NavigateToOrdersCommand { get; }
        public ICommand NavigateToProfileCommand { get; }
        public ICommand PayOrderCommand { get; }

        public OrderViewModel(DatabaseService databaseService,
                              INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            LoadOrdersCommand = new AsyncCommand(LoadOrdersAsync);
            ViewOrderDetailsCommand = new Command<Order>(OnViewOrderDetails);
            NavigateToCatalogCommand = new Command(OnNavigateToCatalog);

            NavigateToBuildPcCommand = new Command(async () =>
                await _navigationService.NavigateToAsync(Routes.BuildPcPage));

            NavigateToCartCommand = new Command(async () =>
                await _navigationService.NavigateToAsync(Routes.CartPage));

            NavigateToOrdersCommand = new Command(async () =>
                await _navigationService.NavigateToAsync(Routes.OrdersPage));

            NavigateToProfileCommand = new Command(async () =>
                await _navigationService.NavigateToAsync(Routes.ProfilePage));

            PayOrderCommand = new Command<Order>(OnPayOrder);

            StatusFilterOptions.Add("Все");
            StatusFilterOptions.Add("Ожидает оплаты");
            StatusFilterOptions.Add("В обработке");
            StatusFilterOptions.Add("Выполнен");

            SelectedStatusFilter = "Все";
        }

        public async Task InitializeAsync(int userId)
        {
            await LoadOrdersAsync();
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                IsBusy = true;
                Orders.Clear();

                if (App.CurrentUser != null)
                {
                    var orders = await _databaseService.GetOrdersByUserAsync(App.CurrentUser.UserId);

                    foreach (var order in orders)
                    {
                        if (SelectedStatusFilter != null && SelectedStatusFilter != "Все")
                        {
                            if (!order.Status.ToString().Contains(SelectedStatusFilter, StringComparison.OrdinalIgnoreCase))
                                continue;
                        }

                        Console.WriteLine($"DEBUG VM LOAD: Id={order.Id}, Total={order.TotalAmount}");

                        Orders.Add(order);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG VM LOAD ERROR: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnViewOrderDetails(Order order)
        {
            if (order == null) return;

            Console.WriteLine($"DEBUG VM DETAILS: order.Id={order.Id}, Total={order.TotalAmount}");

            var parameters = new Dictionary<string, object>
            {
                { "OrderId", order.Id }
            };

            await _navigationService.NavigateToAsync(Routes.OrderDetailPage, parameters);
        }

        private async void OnNavigateToCatalog()
        {
            await _navigationService.NavigateToAsync(Routes.MainPage);
        }

        private async void OnPayOrder(Order? order)
        {
            if (order == null) return;
            await Task.CompletedTask;
        }
    }
}
