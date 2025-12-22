using System.Collections.ObjectModel;
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

        public ObservableCollection<Order> Orders { get; } = new();
        public ICommand LoadOrdersCommand { get; }
        public ICommand ViewOrderDetailsCommand { get; }
        public ICommand NavigateToCatalogCommand { get; }

        public OrderViewModel(DatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            LoadOrdersCommand = new AsyncCommand(LoadOrdersAsync);
            ViewOrderDetailsCommand = new Command<Order>(OnViewOrderDetails);
            NavigateToCatalogCommand = new Command(OnNavigateToCatalog);
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
                if (App.CurrentUser != null)
                {
                    var orders = await _databaseService.GetOrdersByUserAsync(App.CurrentUser.UserId);
                    Orders.Clear();
                    foreach (var order in orders)
                    {
                        Orders.Add(order);
                    }
                }
                else
                {
                    Orders.Add(new Order
                    {
                        Id = 1001,
                        TotalAmount = 125000,
                        OrderDate = DateTime.Now.AddDays(-2),
                        Status = OrderStatus.Processing
                    });
                    Orders.Add(new Order
                    {
                        Id = 1002,
                        TotalAmount = 85000,
                        OrderDate = DateTime.Now.AddDays(-5),
                        Status = OrderStatus.Completed
                    });
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

        private async void OnViewOrderDetails(Order order)
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
    }
}
