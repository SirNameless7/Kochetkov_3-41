using System.Collections.ObjectModel;
using System.Windows.Input;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Constants;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly INavigationService _navigationService;
        private readonly CartService _cartService;

        public ObservableCollection<PcItem> PcItems { get; } = new();
        public ObservableCollection<ComponentCategory> Categories { get; } = new();

        public ICommand LoadDataCommand { get; }
        public ICommand SelectPcCommand { get; }
        public ICommand BuildPcCommand { get; }
        public ICommand NavigateToCartCommand { get; }
        public ICommand AddToCartCommand { get; }

        public MainPageViewModel(DatabaseService databaseService, INavigationService navigationService, CartService cartService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
            _cartService = cartService;

            LoadDataCommand = new AsyncCommand(LoadDataAsync);
            SelectPcCommand = new Command<PcItem>(OnSelectPc);
            BuildPcCommand = new Command(OnBuildPc);
            NavigateToCartCommand = new Command(OnNavigateToCart);
            AddToCartCommand = new Command<PcItem>(OnAddToCart);
        }
        private async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                PcItems.Clear();
                var pcs = await _databaseService.GetPcsAsync();
                foreach (var pc in pcs)
                {
                    PcItems.Add(pc);
                }
                await Task.Delay(100);
                Categories.Clear();
                var categories = await _databaseService.GetComponentCategoriesAsync();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                PcItems.Add(new PcItem
                {
                    Id = 1,
                    Name = "Игровой ПК Start",
                    Price = 60000,
                    Description = "Базовый игровой компьютер"
                });
                Categories.Add(new ComponentCategory { CategoryCode = "CPU", CategoryName = "Процессор" });

                await Application.Current.MainPage.DisplayAlert("Ошибка загрузки", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnSelectPc(PcItem pc)
        {
            if (pc == null) return;

            var parameters = new Dictionary<string, object>
            {
                { "Pc", pc }
            };
            await _navigationService.NavigateToAsync(Routes.PcDetailPage, parameters);
        }

        private async void OnBuildPc()
        {
            await _navigationService.NavigateToAsync(Routes.BuildPcPage);
        }

        private async void OnNavigateToCart()
        {
            await _navigationService.NavigateToAsync(Routes.CartPage);
        }

        private async void OnAddToCart(PcItem pc)
        {
            if (pc == null) return;

            _cartService.AddItem(new CartItem
            {
                Pc = pc,
                Quantity = 1,
                IsCustomBuild = false
            });
            await Application.Current.MainPage.DisplayAlert("Корзина", $"{pc.Name} добавлен в корзину!", "OK");
        }
        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }
    }
}
