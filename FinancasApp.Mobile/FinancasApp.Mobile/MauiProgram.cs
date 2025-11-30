using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.Auth;
using FinancasApp.Mobile.Services.Database;
using FinancasApp.Mobile.Services.LocalDatabase;
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
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

        LiveCharts.Configure(c => c.AddSkiaSharp().AddDefaultMappers().AddLightTheme());
        builder.Logging.AddDebug();
#if DEBUG
        builder.Logging.AddConsole();
#endif

        // === HTTP + REFIT ===
        builder.Services.AddHttpClient("ApiClient", c =>
        {
            c.BaseAddress = new Uri("https://localhost:7001");
            c.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddRefitClient<IApiService>(new RefitSettings
        {
            AuthorizationHeaderValueGetter = async (_, ct) =>
                await SecureStorage.Default.GetAsync("jwt_token") ?? string.Empty
        })
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7001"))
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

        // === SQLITE ===
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "financas.db3");
        builder.Services.AddSingleton<SQLiteAsyncConnection>(sp =>
        {
            var conn = new SQLiteAsyncConnection(dbPath);
            conn.CreateTablesAsync<
                AccountLocal, TransactionLocal, CreditCardLocal, InvoiceLocal,
                TagLocal, TagAssignmentLocal>().Wait();
            return conn;
        });

        // === REPOSITÓRIOS ESPECÍFICOS (obrigatórios para injeção correta) ===
        builder.Services.AddScoped<InvoiceLocalRepository>();
        builder.Services.AddScoped<CreditCardLocalRepository>();
        builder.Services.AddScoped<TransactionLocalRepository>();
        builder.Services.AddScoped<TagLocalRepository>();
        builder.Services.AddScoped<TagAssignmentLocalRepository>();
        builder.Services.AddScoped<ITransactionApiService, TransactionApiService>();
        builder.Services.AddSingleton<ITransactionSyncService, TransactionSyncService>();

        // === SERVIÇOS ===
        builder.Services.AddSingleton<ILocalStorageService, SQLiteStorageService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ISyncService, SyncService>();
        builder.Services.AddSingleton<ITransactionSyncService, TransactionSyncService>();

        // === SYNC SERVICES ===
        builder.Services.AddSingleton<InvoiceSyncService>();  // ← sem interface por enquanto
        // builder.Services.AddSingleton<ITransactionSyncService, TransactionSyncService>(); // comente até criar o arquivo

        // === VIEWMODELS E VIEWS (mantidos) ===
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        // ... (o resto igual)

        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        return builder.Build();
    }
}