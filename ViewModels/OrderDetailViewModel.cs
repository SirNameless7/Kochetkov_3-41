using System.Collections.ObjectModel;
using System.Windows.Input;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using Microsoft.Maui.Graphics;

namespace KPO_Cursovoy.ViewModels
{
    public class OrderDetailViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly INavigationService _navigationService;

        public Order Order { get; set; } = new();
        public ObservableCollection<OrderComponent> OrderItems { get; } = new();
        public ObservableCollection<OrderService> Services { get; } = new();
        public ObservableCollection<StatusHistoryItem> StatusHistory { get; } = new();

        public bool HasServices => Services.Count > 0;
        public bool IsPaymentSectionVisible => Order.Status == OrderStatus.WaitingPayment;
        public bool CanCancelOrder => Order.Status == OrderStatus.New || Order.Status == OrderStatus.WaitingPayment;

        public List<string> PaymentMethods { get; } = new() { "Наличные", "Перевод" };
        public List<string> PaymentTypes { get; } = new() { "Сразу", "Рассрочка" };

        private string _selectedPaymentMethod = "Наличные";
        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set => SetProperty(ref _selectedPaymentMethod, value);
        }

        private string _selectedPaymentType = "Сразу";
        public string SelectedPaymentType
        {
            get => _selectedPaymentType;
            set => SetProperty(ref _selectedPaymentType, value);
        }

        public ICommand PayOrderCommand { get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand BackCommand { get; }

        public OrderDetailViewModel(DatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            PayOrderCommand = new AsyncCommand(OnPayOrder);
            CancelOrderCommand = new Command(OnCancelOrder);
            BackCommand = new Command(OnBack);
        }

        public async Task InitializeAsync(int orderId)
        {
            try
            {
                IsBusy = true;
                Order = new Order
                {
                    Id = orderId,
                    TotalAmount = 125000,
                    OrderDate = DateTime.Now.AddDays(-2),
                    Status = OrderStatus.Processing
                };

                OrderItems.Add(new OrderComponent
                {
                    Component = new ComponentItem { Name = "Intel Core i7-13700K", Price = 45000 },
                    Quantity = 1
                });
                OrderItems.Add(new OrderComponent
                {
                    Component = new ComponentItem { Name = "NVIDIA RTX 4080", Price = 80000 },
                    Quantity = 1
                });
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnPayOrder()
        {
            try
            {
                IsBusy = true;
                Order.Status = OrderStatus.Paid;
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnCancelOrder()
        {
            try
            {
                IsBusy = true;
                Order.Status = OrderStatus.Cancelled;
                await _navigationService.GoBackAsync();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnBack()
        {
            await _navigationService.GoBackAsync();
        }
    }

    public class StatusHistoryItem
    {
        public DateTime Timestamp { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public Color StatusColor { get; set; }
    }
}
