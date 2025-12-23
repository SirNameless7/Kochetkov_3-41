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

                var orders = await _databaseService.GetOrdersByUserAsync(App.CurrentUser.UserId);

                Order = orders.FirstOrDefault(o => o.Id == orderId) ?? new Order { Id = orderId };

                OrderItems.Clear();
                foreach (var oc in Order.Components)
                {
                    OrderItems.Add(oc);
                }

                Services.Clear();
                foreach (var os in Order.Services)
                {
                    Services.Add(os);
                }

                StatusHistory.Clear();
                var allStatuses = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();
                foreach (var status in allStatuses)
                {
                    if (status <= Order.Status)
                    {
                        StatusHistory.Add(new StatusHistoryItem
                        {
                            StatusText = status.ToString(),
                            StatusColor = status == Order.Status ? Colors.Blue : Colors.Gray,
                            Timestamp = Order.OrderDate.AddHours((int)status * 5) 
                        });
                    }
                }

                Order.TotalAmount = Order.Components.Sum(c => c.Component.Price * c.Quantity)
                                    + Order.Services.Sum(s => s.Service.Price);
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
