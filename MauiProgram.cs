using KPO_Cursovoy.Services;
using KPO_Cursovoy.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Npgsql;

namespace KPO_Cursovoy
{
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
            //Менять ТУТ, ОК? Причем, Username и Password от своей бд в постгрессе
            const string connectionString =
                "Host=localhost;Database=App3;Username=postgres;Password=12345";

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            builder.Services.AddSingleton<CartService>();
            builder.Services.AddScoped<DatabaseService>();
            builder.Services.AddScoped<AuthenticationService>();
            builder.Services.AddScoped<CompatibilityService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddTransient<AppShell>();

            builder.Services.AddTransient<StartPageViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();
            builder.Services.AddTransient<BuildPcViewModel>();
            builder.Services.AddTransient<CartViewModel>();
            builder.Services.AddTransient<OrderViewModel>();
            builder.Services.AddTransient<ServicesViewModel>();
            builder.Services.AddTransient<PcDetailViewModel>();
            builder.Services.AddTransient<OrderDetailViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<AdminViewModel>();
            builder.Services.AddTransient<ReportsViewModel>();

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
                        await Application.Current.MainPage.DisplayAlert(
                            "Ошибка БД",
                            "Не удалось подключиться к PostgreSQL.\n\n" +
                            "✓ Запустите сервер PostgreSQL\n" +
                            "✓ Проверьте строку подключения выше\n" +
                            "✓ Пароль верный?",
                            "OK");
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database creation error: {ex.Message}");
                }
            });

            return app;
        }
    }
}
