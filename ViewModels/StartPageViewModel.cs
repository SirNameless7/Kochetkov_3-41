using System.Windows.Input;
using KPO_Cursovoy.Services;
using KPO_Cursovoy.Constants;

namespace KPO_Cursovoy.ViewModels
{
    public class StartPageViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ContinueAsGuestCommand { get; }

        public StartPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            LoginCommand = new Command(OnLogin);
            RegisterCommand = new Command(OnRegister);
            ContinueAsGuestCommand = new Command(OnContinueAsGuest);
        }

        private async void OnLogin()
        {
            await _navigationService.NavigateToAsync(Routes.LoginPage);
        }

        private async void OnRegister()
        {
            await _navigationService.NavigateToAsync(Routes.RegisterPage);
        }

        private async void OnContinueAsGuest()
        {
            await _navigationService.NavigateToAsync(Routes.MainPage);
        }
    }
}
