using FinancasApp.Mobile.Services.Auth;
using FinancasApp.Mobile.Services.Database;
using FinancasApp.Mobile.Services.Navigation;
using FinancasApp.Mobile.Services.Storage;
using FinancasApp.Mobile.Services.Sync;
using FinancasApp.Mobile.ViewModels;
using FinancasApp.Mobile.Views;
using FinancasApp.Mobile.Views.Accounts;
using FinancasApp.Mobile.Views.Auth;
using FinancasApp.Mobile.Views.CreditCards;
using FinancasApp.Mobile.Views.Dashboard;
using FinancasApp.Mobile.Views.Reports;
using FinancasApp.Mobile.Views.Transactions;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Refit;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SQLite;

namespace FinancasApp.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        LiveCharts.Configure(config =>
            config.AddSkiaSharp().AddDefaultMappers().AddLightTheme()
        );

        builder.Logging.AddDebug();
#if DEBUG
        builder.Logging.AddConsole();
#endif

        builder.Services.AddHttpClient("RefitClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7001");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddRefitClient<IApiService>(new RefitSettings
        {
            AuthorizationHeaderValueGetter = async (_, __) =>
                await SecureStorage.GetAsync("jwt_token") ?? string.Empty

        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler());

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "financas.db3");
        var connection = new SQLiteAsyncConnection(dbPath);

        DatabaseInitializer.InitializeAsync(connection).Wait();

        builder.Services.AddSingleton(connection);
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ILocalStorageService, SQLiteStorageService>();


        // ✔ SyncService registrado como ISyncService
        builder.Services.AddSingleton<ISyncService, SyncService>();
        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();



        builder.UseMauiApp<App>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<AccountsViewModel>();
        builder.Services.AddTransient<NewTransactionViewModel>();
        builder.Services.AddTransient<CreditCardsViewModel>();
        builder.Services.AddTransient<InvoiceDetailViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<AccountsPage>();
        builder.Services.AddTransient<NewTransactionPage>();
        builder.Services.AddTransient<CreditCardsPage>();
        builder.Services.AddTransient<InvoiceDetailPage>();
        builder.Services.AddTransient<ReportsPage>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddSingleton<AppShell>();


        return builder.Build();
    }
}
