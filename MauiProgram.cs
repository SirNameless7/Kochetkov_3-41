using KPO_Cursovoy.Services;
using KPO_Cursovoy.ViewModels;
using KPO_Cursovoy.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Hosting;
using Npgsql;

namespace KPO_Cursovoy;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var connectionString =
            Environment.GetEnvironmentVariable("KPO_CURSOVOY_CONNECTION")
            ?? "Host=localhost;Database=App12;Username=postgres;Password=bognebes";

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        //Services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddScoped<AuthenticationService>();

        builder.Services.AddSingleton<CartService>();
        builder.Services.AddSingleton<CompatibilityService>();
        builder.Services.AddSingleton<PaymentService>();
        builder.Services.AddSingleton<OrderProcessingService>();
        builder.Services.AddSingleton<StockService>();
        builder.Services.AddSingleton<AnalyticsService>();
        builder.Services.AddSingleton<LoyaltyService>();

        builder.Services.AddSingleton<INavigationService, NavigationService>();

        //ViewModels
        builder.Services.AddTransient<StartPageViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();

        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<PcDetailViewModel>();
        builder.Services.AddTransient<CartViewModel>();
        builder.Services.AddTransient<BuildPcViewModel>();

        builder.Services.AddTransient<OrderViewModel>();
        builder.Services.AddTransient<OrderDetailViewModel>();

        builder.Services.AddTransient<ServicesViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<AdminViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();

        //Pages
        builder.Services.AddTransient<StartPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<PcDetailPage>();
        builder.Services.AddTransient<CartPage>();
        builder.Services.AddTransient<BuildPcPage>();

        builder.Services.AddTransient<OrdersPage>();
        builder.Services.AddTransient<ServicesPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<AdminPage>();
        builder.Services.AddTransient<ReportsPage>();

        //Shell
        builder.Services.AddSingleton<AppShell>();

        var app = builder.Build();

        Task.Run(async () =>
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var databaseService = scope.ServiceProvider.GetRequiredService<DatabaseService>();
                await databaseService.InitializeAsync();
            }
            catch (NpgsqlException)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var page = Application.Current?.MainPage;
                    if (page == null) return;

                    await page.DisplayAlert(
                        "Ошибка БД",
                        "Не удалось подключиться к PostgreSQL.\n\n" +
                        "✓ Запустите сервер PostgreSQL\n" +
                        "✓ Проверьте строку подключения (или переменную KPO_CURSOVOY_CONNECTION)\n" +
                        "✓ Пароль верный?",
                        "OK");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database creation error: {ex}");
            }
        });

        return app;
    }
}
