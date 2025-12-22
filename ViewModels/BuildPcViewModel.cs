using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KPO_Cursovoy.Models;
using KPO_Cursovoy.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace KPO_Cursovoy.ViewModels
{
    public class BuildPcViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly DatabaseService _databaseService;
        private readonly CartService _cartService;
        private readonly CompatibilityService _compatibilityService;

        public ObservableCollection<ComponentCategory> Categories { get; } = new();
        public ObservableCollection<ComponentItem> AvailableComponents { get; } = new();
        public ObservableCollection<ComponentItem> SelectedComponents { get; } = new();

        private ComponentCategory? _selectedCategory;
        public ComponentCategory? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                IsComponentSelectionVisible = value != null;
                if (value != null)
                {
                    LoadAvailableComponentsAsync();
                }
                else
                {
                    AvailableComponents.Clear();
                }
            }
        }

        private bool _isComponentSelectionVisible;
        public bool IsComponentSelectionVisible
        {
            get => _isComponentSelectionVisible;
            set => SetProperty(ref _isComponentSelectionVisible, value);
        }

        private string _compatibilityStatus = "Не проверено";
        public string CompatibilityStatus
        {
            get => _compatibilityStatus;
            set => SetProperty(ref _compatibilityStatus, value);
        }

        private string _compatibilityResult = "Выберите компоненты для проверки";
        public string CompatibilityResult
        {
            get => _compatibilityResult;
            set => SetProperty(ref _compatibilityResult, value);
        }

        private Microsoft.Maui.Graphics.Color _compatibilityResultColor = Colors.Gray;
        public Microsoft.Maui.Graphics.Color CompatibilityResultColor
        {
            get => _compatibilityResultColor;
            set => SetProperty(ref _compatibilityResultColor, value);
        }

        private bool _isCompatible;
        public bool IsCompatible
        {
            get => _isCompatible;
            set => SetProperty(ref _isCompatible, value);
        }

        private bool _isCompatibilityResultVisible;
        public bool IsCompatibilityResultVisible
        {
            get => _isCompatibilityResultVisible;
            set => SetProperty(ref _isCompatibilityResultVisible, value);
        }

        public decimal TotalPrice => SelectedComponents.Sum(c => c.Price);

        public ICommand LoadCategoriesCommand { get; }
        public ICommand SelectComponentCommand { get; }
        public ICommand RemoveComponentCommand { get; }
        public ICommand CheckCompatibilityCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand RefreshCommand { get; }

        public BuildPcViewModel(
            INavigationService navigationService,
            DatabaseService databaseService,
            CartService cartService,
            CompatibilityService compatibilityService)
        {
            _navigationService = navigationService;
            _databaseService = databaseService;
            _cartService = cartService;
            _compatibilityService = compatibilityService;

            LoadCategoriesCommand = new AsyncCommand(LoadCategoriesAsync);
            SelectComponentCommand = new Command<ComponentItem>(SelectComponent);
            RemoveComponentCommand = new Command<ComponentItem>(RemoveComponent);
            CheckCompatibilityCommand = new AsyncCommand(CheckCompatibilityAsync);
            AddToCartCommand = new Command(AddToCart);
            RefreshCommand = new AsyncCommand(RefreshAsync);

            SelectedComponents.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TotalPrice));
                IsCompatibilityResultVisible = SelectedComponents.Count > 1;
            };
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                IsBusy = true;
                Categories.Clear();
                var categories = await _databaseService.GetComponentCategoriesAsync();

                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                if (Categories.Count > 0)
                {
                    SelectedCategory = Categories.First();
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить категории: {ex.Message}", "ОК");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadAvailableComponentsAsync()
        {
            if (_selectedCategory == null)
            {
                AvailableComponents.Clear();
                return;
            }

            try
            {
                IsBusy = true;
                AvailableComponents.Clear();
                var components = await _databaseService.GetComponentsByCategoryAsync(_selectedCategory.CategoryCode);

                foreach (var component in components)
                {
                    AvailableComponents.Add(component);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить компоненты: {ex.Message}", "ОК");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SelectComponent(ComponentItem component)
        {
            if (component == null) return;

            var existing = SelectedComponents.FirstOrDefault(c => c.CategoryCode == component.CategoryCode);
            if (existing != null)
            {
                SelectedComponents.Remove(existing);
            }

            SelectedComponents.Add(component);
            CompatibilityStatus = "Не проверено";
        }

        private void RemoveComponent(ComponentItem component)
        {
            if (component == null) return;
            SelectedComponents.Remove(component);
            CompatibilityStatus = "Не проверено";
        }

        private async Task CheckCompatibilityAsync()
        {
            if (SelectedComponents.Count < 2)
            {
                await Application.Current.MainPage.DisplayAlert("Проверка совместимости",
                    "Выберите минимум 2 компонента для проверки", "ОК");
                return;
            }

            try
            {
                IsBusy = true;
                var result = await _compatibilityService.CheckCompatibilityAsync(SelectedComponents.ToList());

                if (result.IsCompatible)
                {
                    CompatibilityResult = "Все компоненты совместимы!";
                    CompatibilityResultColor = Colors.Green;
                    CompatibilityStatus = "Совместимо";
                    IsCompatible = true;
                }
                else
                {
                    var reasons = string.Join("\n", result.IncompatiblePairs.Select(p =>
                        $"- {p.Component1} и {p.Component2}: {p.Reason}"));

                    CompatibilityResult = $"Несовместимые компоненты:\n{reasons}";
                    CompatibilityResultColor = Colors.Red;
                    CompatibilityStatus = "Несовместимо";
                    IsCompatible = false;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось проверить совместимость: {ex.Message}", "ОК");
                CompatibilityResult = "Ошибка при проверке совместимости";
                CompatibilityResultColor = Colors.Red;
                CompatibilityStatus = "Ошибка";
                IsCompatible = false;
            }
            finally
            {
                IsBusy = false;
                IsCompatibilityResultVisible = true;
            }
        }

        private void AddToCart()
        {
            if (!IsCompatible)
            {
                Application.Current.MainPage.DisplayAlert("Ошибка", "Невозможно добавить в корзину: есть несовместимые компоненты", "ОК");
                return;
            }

            if (SelectedComponents.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert("Корзина", "Выберите компоненты для сборки ПК", "ОК");
                return;
            }

            var customPc = new PcItem
            {
                Name = $"Собранный ПК ({DateTime.Now:dd.MM.yyyy})",
                Description = "Собран по вашему заказу",
                Price = TotalPrice,
                Components = new List<ComponentItem>(SelectedComponents)
            };

            _cartService.AddItem(new CartItem
            {
                Pc = customPc,
                Quantity = 1,
                IsCustomBuild = true
            });

            Application.Current.MainPage.DisplayAlert("Корзина", "Собранный ПК добавлен в корзину", "ОК");
        }

        private async Task RefreshAsync()
        {
            await LoadCategoriesAsync();
        }

        public async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
        }
    }
}
