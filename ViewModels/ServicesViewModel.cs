using System.Collections.ObjectModel;
using System.Windows.Input;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;

namespace KPO_Cursovoy.ViewModels
{
    public class ServicesViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<ServiceItem> Services { get; } = new();
        public ICommand LoadServicesCommand { get; }
        public ICommand SelectServiceCommand { get; }

        public ServicesViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
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
                Services.Add(new ServiceItem
                {
                    Name = "Сборка ПК",
                    Description = "Профессиональная сборка компьютера",
                    Price = 3000
                });
                Services.Add(new ServiceItem
                {
                    Name = "Тестирование",
                    Description = "Полное тестирование всех компонентов",
                    Price = 1500
                });
                Services.Add(new ServiceItem
                {
                    Name = "Доставка",
                    Description = "Доставка по городу в день заказа",
                    Price = 500
                });
                Services.Add(new ServiceItem
                {
                    Name = "Гарантия +1 год",
                    Description = "Расширение гарантии на 1 год",
                    Price = 5000
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

        private void OnSelectService(ServiceItem service)
        {
            if (service == null) return;
        }
    }
}
