using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace KPO_Cursovoy.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
