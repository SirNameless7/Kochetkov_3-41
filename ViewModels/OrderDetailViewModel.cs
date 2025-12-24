using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        private Order _order = new();
        public Order Order
        {
            get => _order;
            set
            {
                if (SetProperty(ref _order, value))
                {
                    OnPropertyChanged(nameof(IsPaymentSectionVisible));
                    OnPropertyChanged(nameof(CanCancelOrder));
                }
            }
        }

        public ObservableCollection<OrderItemLine> OrderItems { get; } = new();
        public ObservableCollection<ServiceLine> Services { get; } = new();
        public ObservableCollection<StatusHistoryItem> StatusHistory { get; } = new();

        public bool HasServices => Services.Count > 0;
        public bool IsPaymentSectionVisible => Order.Status == OrderStatus.WaitingPayment;
        public bool CanCancelOrder => Order.Status == OrderStatus.New || Order.Status == OrderStatus.WaitingPayment;

        public ICommand PayOrderCommand { get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand BackCommand { get; }

        public OrderDetailViewModel(DatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            PayOrderCommand = new AsyncCommand(OnPayOrder);
            CancelOrderCommand = new AsyncCommand(OnCancelOrder);
            BackCommand = new Command(OnBack);
        }

        public async Task InitializeAsync(int orderId)
        {
            try
            {
                IsBusy = true;

                if (App.CurrentUser == null)
                {
                    Order = new Order { Id = orderId };
                    OrderItems.Clear();
                    Services.Clear();
                    StatusHistory.Clear();
                    OnPropertyChanged(nameof(HasServices));
                    return;
                }

                var orders = await _databaseService.GetOrdersByUserAsync(App.CurrentUser.UserId);
                var found = orders.FirstOrDefault(o => o.Id == orderId);

                if (found == null)
                {
                    Order = new Order { Id = orderId };
                    OrderItems.Clear();
                    Services.Clear();
                    StatusHistory.Clear();
                    OnPropertyChanged(nameof(HasServices));
                    return;
                }

                Order = found;

                // Услуги (ObservableCollection для UI)
                Services.Clear();
                foreach (var os in Order.Services)
                {
                    Services.Add(new ServiceLine
                    {
                        Name = os.Service.Name,
                        Description = os.Service.Description,
                        Price = os.Service.Price
                    });
                }
                OnPropertyChanged(nameof(HasServices));

                // Состав (либо готовый ПК, либо комплектующие)
                OrderItems.Clear();

                decimal itemsSum = 0m;

                if (Order.PcId.HasValue && !Order.IsCustomBuild && (Order.Components == null || Order.Components.Count == 0))
                {
                    var pc = await _databaseService.GetPcByIdAsync(Order.PcId.Value);
                    if (pc != null)
                    {
                        OrderItems.Add(new OrderItemLine
                        {
                            Name = pc.Name,
                            CategoryCode = "Готовый ПК",
                            Price = pc.Price,
                            Quantity = 1
                        });

                        itemsSum = pc.Price;
                    }
                }
                else
                {
                    foreach (var oc in Order.Components)
                    {
                        OrderItems.Add(new OrderItemLine
                        {
                            Name = oc.Component.Name,
                            CategoryCode = oc.Component.CategoryCode,
                            Price = oc.Component.Price,
                            Quantity = oc.Quantity
                        });
                    }

                    itemsSum = Order.Components.Sum(c => c.Component.Price * c.Quantity);
                }

                var servicesSum = Services.Sum(s => s.Price);

                Order.TotalAmount = itemsSum + servicesSum;
                OnPropertyChanged(nameof(Order));

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

                OnPropertyChanged(nameof(IsPaymentSectionVisible));
                OnPropertyChanged(nameof(CanCancelOrder));
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

                await _databaseService.UpdateOrderStatusAsync(Order.Id, OrderStatus.Paid);
                Order.Status = OrderStatus.Paid;

                OnPropertyChanged(nameof(Order));
                OnPropertyChanged(nameof(IsPaymentSectionVisible));
                OnPropertyChanged(nameof(CanCancelOrder));

                await _navigationService.GoBackAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnCancelOrder()
        {
            try
            {
                IsBusy = true;

                await _databaseService.UpdateOrderStatusAsync(Order.Id, OrderStatus.Cancelled);
                Order.Status = OrderStatus.Cancelled;

                OnPropertyChanged(nameof(Order));
                OnPropertyChanged(nameof(IsPaymentSectionVisible));
                OnPropertyChanged(nameof(CanCancelOrder));

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

    public class OrderItemLine
    {
        public string Name { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => Price * Quantity;
    }

    public class ServiceLine
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class StatusHistoryItem
    {
        public DateTime Timestamp { get; set; }
        public string StatusText { get; set; } = string.Empty;
        public Color StatusColor { get; set; }
    }
}
