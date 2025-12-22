using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using Microsoft.Maui.Graphics;
using KPO_Cursovoy.Constants;
//Не забыть додделать
namespace KPO_Cursovoy.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly DatabaseService _databaseService;

        private int _todayOrdersCount = 15;
        public int TodayOrdersCount
        {
            get => _todayOrdersCount;
            set => SetProperty(ref _todayOrdersCount, value);
        }

        private decimal _todayRevenue = 245000;
        public decimal TodayRevenue
        {
            get => _todayRevenue;
            set => SetProperty(ref _todayRevenue, value);
        }

        private int _newUsersCount = 8;
        public int NewUsersCount
        {
            get => _newUsersCount;
            set => SetProperty(ref _newUsersCount, value);
        }

        public bool HasLowStockItems => LowStockItems.Any();

        public ObservableCollection<Order> ActiveOrders { get; } = new();
        public ObservableCollection<ComponentItem> LowStockItems { get; } = new();
        public bool IsNotBusy => !IsBusy;

        public ICommand RefreshCommand { get; }
        public ICommand ViewReportsCommand { get; }
        public ICommand ViewDownloadsCommand { get; }
        public ICommand ManageComponentsCommand { get; }
        public ICommand ManageUsersCommand { get; }
        public ICommand AnalyticsCommand { get; }
        public ICommand SettingsCommand { get; }

        public AdminViewModel(INavigationService navigationService, DatabaseService databaseService)
        {
            _navigationService = navigationService;
            _databaseService = databaseService;

            RefreshCommand = new AsyncCommand(RefreshDataAsync);
            ViewReportsCommand = new Command(ViewReports);
            ViewDownloadsCommand = new Command(ViewDownloads);
            ManageComponentsCommand = new Command(ManageComponents);
            ManageUsersCommand = new Command(ManageUsers);
            AnalyticsCommand = new Command(Analytics);
            SettingsCommand = new Command(Settings);

            LoadSampleData();
        }

        private void LoadSampleData()
        {
            ActiveOrders.Add(new Order
            {
                Id = 1001,
                UserId = 1,
                TotalAmount = 62000,
                Status = OrderStatus.WaitingPayment
            });

            ActiveOrders.Add(new Order
            {
                Id = 1002,
                UserId = 2,
                TotalAmount = 36500,
                Status = OrderStatus.Processing
            });

            foreach (var order in ActiveOrders)
            {
                order.Components = new List<OrderComponent>();
            }

            LowStockItems.Add(new ComponentItem
            {
                Name = "NVIDIA RTX 3060",
                Stock = 2,
                CategoryName = "Видеокарты"
            });

            LowStockItems.Add(new ComponentItem
            {
                Name = "Intel Core i5-11400",
                Stock = 3,
                CategoryName = "Процессоры"
            });

            TodayOrdersCount = ActiveOrders.Count;
            TodayRevenue = ActiveOrders.Sum(o => o.TotalAmount);
            NewUsersCount = 8;
        }

        private async Task RefreshDataAsync()
        {
            try
            {
                IsBusy = true;
                await Task.Delay(1000);
                ActiveOrders.Clear();
                LoadSampleData();
                OnPropertyChanged(nameof(IsNotBusy));
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        private async void ViewReports()
        {
            await _navigationService.NavigateToAsync(Routes.ReportsPage);
        }

        private void ViewDownloads()
        {
        }

        private void ManageComponents()
        {
        }

        private void ManageUsers()
        {
        }

        private void Analytics()
        {
        }

        private void Settings()
        {
        }
    }
}
