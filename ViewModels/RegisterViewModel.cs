using System.Windows.Input;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Constants;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly AuthenticationService _authService;

        private string _login = "";
        private string _phone = "";
        private string _password = "";
        private string _errorMessage = "";

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
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

        public RegisterViewModel(INavigationService navigationService, AuthenticationService authService)
        {
            _navigationService = navigationService;
            _authService = authService;

            RegisterCommand = new AsyncCommand(RegisterAsync);
            LoginCommand = new Command(OnLogin);
        }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Login) ||
                string.IsNullOrWhiteSpace(Phone) ||
                string.IsNullOrWhiteSpace(Password) ||
                Phone.Length < 11)
            {
                ErrorMessage = "Заполните все поля корректно";
                return;
            }

            IsBusy = true;
            ErrorMessage = "";

            try
            {
                var success = await _authService.RegisterAsync(Login, Phone, Password);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Успех",
                        "Регистрация успешна!", "OK");

                    await _navigationService.NavigateToAsync(Routes.MainPage);
                    return;
                }

                ErrorMessage = "Пользователь с таким логином или телефоном уже существует";
                await Application.Current.MainPage.DisplayAlert("Ошибка", ErrorMessage, "OK");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка регистрации: " + ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void OnLogin()
        {
            await _navigationService.NavigateToAsync(Routes.LoginPage);
        }
    }
}
