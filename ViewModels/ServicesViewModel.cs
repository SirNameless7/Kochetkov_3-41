using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using KPO_Cursovoy.Constants;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.ViewModels
{
    public class ServicesViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly INavigationService _navigationService;

        public ObservableCollection<ServiceItem> Services { get; } = new();

        public ICommand LoadServicesCommand { get; }
        public ICommand SelectServiceCommand { get; }

        public ServicesViewModel(DatabaseService databaseService, INavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;

            LoadServicesCommand = new AsyncCommand(LoadServicesAsync);
            SelectServiceCommand = new Command<ServiceItem>(OnSelectService);
        }

        public async Task InitializeAsync()
        {
            await LoadServicesAsync();
        }

        private async Task LoadServicesAsync()
        {
            try
            {
                IsBusy = true;

                Services.Clear();
                var services = await _databaseService.GetServicesAsync();

                foreach (var s in services)
                    Services.Add(s);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "ОК");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnSelectService(ServiceItem service)
        {
            if (service == null) return;

            var user = App.CurrentUser;
            if (user == null)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Сначала войдите в аккаунт.", "ОК");
                return;
            }

            try
            {
                IsBusy = true;

                var order = new Order
                {
                    UserId = user.UserId,
                    PcId = null,
                    IsCustomBuild = false,
                    TotalAmount = service.Price,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.WaitingPayment
                };

                order.Services.Add(new OrderService
                {
                    ServiceId = service.Id
                });

                await _databaseService.CreateOrderAsync(order, usedComponents: null);

                await Application.Current.MainPage.DisplayAlert(
                    "Услуга",
                    "Заказ на услугу создан. Перейдите в «Заказы» для оплаты.",
                    "ОК");

                await _navigationService.NavigateToAsync(Routes.OrdersPage);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "ОК");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
