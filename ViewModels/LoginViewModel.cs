using System.Windows.Input;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Constants;

namespace KPO_Cursovoy.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly AuthenticationService _authService;

        private string _phone = "";
        private string _password = "";
        private string _errorMessage = "";

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

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(INavigationService navigationService, AuthenticationService authService)
        {
            _navigationService = navigationService;
            _authService = authService;

            LoginCommand = new AsyncCommand(LoginAsync);
            RegisterCommand = new Command(OnRegister);
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Заполните логин и пароль";
                return;
            }

            IsBusy = true;
            ErrorMessage = "";

            try
            {
                var success = await _authService.LoginAsync(Phone, Password);

                if (success)
                {
                    ErrorMessage = "Успешный вход!";
                    await _navigationService.NavigateToAsync(Routes.MainPage);
                    return;
                }

                ErrorMessage = "Неверный логин или пароль";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnRegister()
        {
            await _navigationService.NavigateToAsync(Routes.RegisterPage);
        }

        public Task InitializeAsync() => Task.CompletedTask;
    }
}
