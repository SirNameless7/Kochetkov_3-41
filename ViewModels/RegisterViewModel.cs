using System.Windows.Input;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Constants;

namespace KPO_Cursovoy.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly AuthenticationService _authService;

        private string _fullName = "";
        private string _phone = "";
        private string _password = "";
        private string _errorMessage = "";

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand LoginCommand { get; }

        public RegisterViewModel(INavigationService navigationService, AuthenticationService authService)  // ✅ authService!
        {
            _navigationService = navigationService;
            _authService = authService;

            RegisterCommand = new AsyncCommand(RegisterAsync);
            LoginCommand = new Command(OnLogin);
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(FullName) ||
                string.IsNullOrWhiteSpace(Phone) ||
                string.IsNullOrWhiteSpace(Password) ||
                Phone.Length < 11)
            {
                ErrorMessage = "Заполните все поля корректно";
                return;
            }

            IsBusy = true;
            ErrorMessage = "";
            var success = await _authService.RegisterAsync(Phone, Password, FullName);

            if (success)
            {
                ErrorMessage = "Регистрация успешна!";
                await _navigationService.NavigateToAsync(Routes.MainPage);
            }
            else
            {
                ErrorMessage = "Пользователь с таким телефоном уже существует";
            }

            IsBusy = false;
        }

        private async void OnLogin()
        {
            await _navigationService.NavigateToAsync(Routes.LoginPage);
        }
    }
}
