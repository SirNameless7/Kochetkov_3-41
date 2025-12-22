using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Services
{
    public interface INavigationService
    {
        Task NavigateToAsync(string route);
        Task NavigateToAsync(string route, object parameter);
        Task GoBackAsync();
    }
}