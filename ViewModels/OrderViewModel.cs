using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KPO_Cursovoy.Constants;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Views;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private readonly OrderProcessingService _orderProcessingService;

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
                    LoadOrdersCommand.Execute(null);
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

        public OrderViewModel(
            DatabaseService databaseService,
            INavigationService navigationService,
            OrderProcessingService orderProcessingService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _orderProcessingService = orderProcessingService;

            LoadOrdersCommand = new AsyncCommand(LoadOrdersAsync);
            ViewOrderDetailsCommand = new Command<Order>(OnViewOrderDetails);

            NavigateToCatalogCommand = new Command(OnNavigateToCatalog);
            NavigateToBuildPcCommand = new Command(async () => await _navigationService.NavigateToAsync(Routes.BuildPcPage));
            NavigateToCartCommand = new Command(async () => await _navigationService.NavigateToAsync(Routes.CartPage));
            NavigateToOrdersCommand = new Command(async () => await _navigationService.NavigateToAsync(Routes.OrdersPage));
            NavigateToProfileCommand = new Command(async () => await _navigationService.NavigateToAsync(Routes.ProfilePage));

            PayOrderCommand = new Command<Order>(OnPayOrder);

            StatusFilterOptions.Clear();
            StatusFilterOptions.Add("Все");
            //StatusFilterOptions.Add("Новый");
            StatusFilterOptions.Add("Ожидает оплаты");
            StatusFilterOptions.Add("Оплачен");
            //StatusFilterOptions.Add("В обработке");
            //StatusFilterOptions.Add("Завершен");
            StatusFilterOptions.Add("Отменен");

            // ВАЖНО: НЕ делаем SelectedStatusFilter = "Все";
            // чтобы не вызвать LoadOrdersCommand.Execute(null) прямо в конструкторе
            _selectedStatusFilter = "Все";
            OnPropertyChanged(nameof(SelectedStatusFilter));
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

                if (App.CurrentUser == null)
                    return;

                var orders = await _databaseService.GetOrdersByUserAsync(App.CurrentUser.UserId);

                foreach (var order in orders)
                {
                    if (!PassesFilter(order))
                        continue;

                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadOrders error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool PassesFilter(Order order)
        {
            if (string.IsNullOrWhiteSpace(SelectedStatusFilter) || SelectedStatusFilter == "Все")
                return true;

            return SelectedStatusFilter switch
            {
                //"Новый" => order.Status == OrderStatus.New,
                "Ожидает оплаты" => order.Status == OrderStatus.WaitingPayment,
                "Оплачен" => order.Status == OrderStatus.Paid,
                //"В обработке" => order.Status == OrderStatus.Processing,
                //"Завершен" => order.Status == OrderStatus.Completed,
                "Отменен" => order.Status == OrderStatus.Cancelled,
                _ => true
            };
        }

        private async void OnViewOrderDetails(Order? order)
        {
            if (order == null) return;

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
            if (order == null)
                return;

            var popup = new PaymentPopupPage(order);
            await Application.Current!.MainPage!.Navigation.PushModalAsync(popup);

            var result = await popup.ResultTask;

            if (result == PaymentPopupResult.Closed)
                return;

            if (result == PaymentPopupResult.CancelOrder)
            {
                await _orderProcessingService.CancelOrderAsync(order.Id);
                await LoadOrdersAsync();
                await Application.Current!.MainPage!.DisplayAlert("Заказ отменён", $"Заказ №{order.Id} отменён.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var success = await _orderProcessingService.ProcessPaymentAsync(
                    order.Id,
                    PaymentMethod.Transfer,
                    PaymentType.Full);

                if (!success)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Оплата не прошла", "Платёж отклонён.", "OK");
                    return;
                }

                await Application.Current!.MainPage!.DisplayAlert("Успех", "Оплата прошла успешно.", "OK");
                await LoadOrdersAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
