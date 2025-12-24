using System;
using System.Threading.Tasks;
using System.Windows.Input;
using KPO_Cursovoy.Constants;
using KPO_Cursovoy.Services;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.ViewModels;

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
    public ICommand BackCommand { get; }

    public RegisterViewModel(INavigationService navigationService, AuthenticationService authService)
    {
        _navigationService = navigationService;
        _authService = authService;

        RegisterCommand = new AsyncCommand(RegisterAsync);
        LoginCommand = new Command(OnLogin);
        BackCommand = new Command(async () => await _navigationService.GoBackAsync());
    }

    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Login) ||
            string.IsNullOrWhiteSpace(Phone) ||
            string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Заполните все поля корректно";
            return;
        }

        IsBusy = true;
        ErrorMessage = "";

        try
        {
            var success = await _authService.RegisterAsync(Login.Trim(), Phone.Trim(), Password);
            if (!success)
            {
                ErrorMessage = "Пользователь с таким логином или телефоном уже существует";
                await Application.Current!.MainPage.DisplayAlert("Ошибка", ErrorMessage, "OK");
                return;
            }

            await Application.Current!.MainPage.DisplayAlert("Успех", "Регистрация успешна!", "OK");
            await _navigationService.NavigateToAsync(Routes.MainPage);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Ошибка регистрации: " + ex.Message;
            await Application.Current!.MainPage.DisplayAlert("Ошибка", ErrorMessage, "OK");
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
