using KPO_Cursovoy.Services;
using KPO_Cursovoy.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

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

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = "Host=localhost;Database=App;Username=postgres;Password=12345";
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

            // простая инициализация БД
            Task.Run(async () =>
            {
                try
                {
                    using var scope = app.Services.CreateScope();
                    var databaseService = scope.ServiceProvider.GetRequiredService<DatabaseService>();
                    await databaseService.InitializeAsync();
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
