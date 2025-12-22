using System.Windows.Input;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Constants;

namespace KPO_Cursovoy.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        private string _fullName = "Гость";
        public string FullName
        {
            get => App.CurrentUser?.FullName ?? _fullName;
            set => SetProperty(ref _fullName, value);
        }

        private string _phone = "Не указан";
        public string Phone
        {
            get => App.CurrentUser?.Phone ?? _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _loyaltyStatus = "обычный";
        public string LoyaltyStatus
        {
            get => App.CurrentUser?.LoyaltyStatus ?? _loyaltyStatus;
            set => SetProperty(ref _loyaltyStatus, value);
        }

        public string Initials => GetInitials(FullName);
        public int OrderCount { get; set; } = 5;
        public decimal DiscountPercent { get; set; } = 10;
        public decimal TotalSpent { get; set; } = 150000;
        public DateTime LastVisit { get; set; } = DateTime.Now.AddDays(-3);

        public ICommand ViewOrdersCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand SupportCommand { get; }
        public ICommand LogoutCommand { get; }

        public ProfileViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ViewOrdersCommand = new Command(OnViewOrders);
            SettingsCommand = new Command(OnSettings);
            SupportCommand = new Command(OnSupport);
            LogoutCommand = new Command(OnLogout);
        }

        public void Initialize()
        {
            OnPropertyChanged(nameof(FullName));
            OnPropertyChanged(nameof(Phone));
            OnPropertyChanged(nameof(LoyaltyStatus));
            OnPropertyChanged(nameof(Initials));
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "?";

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return $"{parts[0][0]}{parts[1][0]}";
            }
            return parts[0].Length >= 2 ? parts[0].Substring(0, 2).ToUpper() : "?";
        }

        private async void OnViewOrders()
        {
            await _navigationService.NavigateToAsync(Routes.OrdersPage);
        }

        private async void OnSettings()
        {
        }

        private async void OnSupport()
        {
        }

        private async void OnLogout()
        {
            App.CurrentUser = null;
            await _navigationService.NavigateToAsync(Routes.StartPage);
        }
    }
}
