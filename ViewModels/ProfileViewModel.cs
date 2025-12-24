using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KPO_Cursovoy.Constants;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly DatabaseService _databaseService;

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

    private int _orderCount;
    public int OrderCount
    {
        get => _orderCount;
        set => SetProperty(ref _orderCount, value);
    }

    private decimal _discountPercent;
    public decimal DiscountPercent
    {
        get => _discountPercent;
        set => SetProperty(ref _discountPercent, value);
    }

    private decimal _totalSpent;
    public decimal TotalSpent
    {
        get => _totalSpent;
        set => SetProperty(ref _totalSpent, value);
    }

    private DateTime? _lastVisit;
    public DateTime? LastVisit
    {
        get => _lastVisit;
        set => SetProperty(ref _lastVisit, value);
    }

    public ICommand ViewOrdersCommand { get; }
    public ICommand SettingsCommand { get; }
    public ICommand SupportCommand { get; }
    public ICommand LogoutCommand { get; }

    public ProfileViewModel(INavigationService navigationService, DatabaseService databaseService)
    {
        _navigationService = navigationService;
        _databaseService = databaseService;

        ViewOrdersCommand = new Command(OnViewOrders);
        SettingsCommand = new Command(OnSettings);
        SupportCommand = new Command(OnSupport);
        LogoutCommand = new Command(OnLogout);
    }

    public async Task InitializeAsync()
    {
        OnPropertyChanged(nameof(FullName));
        OnPropertyChanged(nameof(Phone));
        OnPropertyChanged(nameof(LoyaltyStatus));
        OnPropertyChanged(nameof(Initials));

        await LoadStatsAsync();
    }

    private async Task LoadStatsAsync()
    {
        if (App.CurrentUser == null)
        {
            OrderCount = 0;
            TotalSpent = 0;
            DiscountPercent = 0;
            LastVisit = null;
            LoyaltyStatus = "обычный";
            return;
        }

        var orders = await _databaseService.GetOrdersByUserAsync(App.CurrentUser.UserId);

        var paidOrders = orders
            .Where(o => o.Status is OrderStatus.Paid or OrderStatus.Completed)
            .ToList();

        OrderCount = paidOrders.Count;

        LastVisit = paidOrders.Count > 0 ? paidOrders.Max(o => o.OrderDate) : null;

        var paidOrdersCount = paidOrders.Count;
        var totalSpentPaid = paidOrders.Sum(o => o.TotalAmount);

        TotalSpent = totalSpentPaid;

        var actualStatus = await _databaseService.UpdateUserLoyaltyFromMetricsAsync(
            App.CurrentUser.UserId,
            totalSpentPaid,
            paidOrdersCount);

        App.CurrentUser.LoyaltyStatus = actualStatus;
        LoyaltyStatus = actualStatus;

        DiscountPercent = DatabaseService.GetDiscountPercentByStatus(actualStatus);
    }

    private static string GetInitials(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return "?";

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[1][0]}";

        return parts[0].Length >= 2 ? parts[0].Substring(0, 2).ToUpper() : "?";
    }

    private async void OnViewOrders()
    {
        await _navigationService.NavigateToAsync(Routes.OrdersPage);
    }

    private async void OnSettings()
    {
        await Task.CompletedTask;
    }

    private async void OnSupport()
    {
        await Task.CompletedTask;
    }

    private void OnLogout()
    {
        if (Application.Current is App app)
            app.Logout();
    }
}
