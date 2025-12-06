// MauiProgram.cs — VERSÃO FINAL, IMORTAL E COMPILÁVEL (.NET 9 – 06/12/2025)
using System.Text.Json;
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
using Microsoft.Extensions.Http.Resilience; // ← ESSA LINHA É OBRIGATÓRIA
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

        LiveCharts.Configure(c => c
            .AddSkiaSharp()
            .AddDefaultMappers()
            .AddLightTheme());

        builder.Logging.AddDebug();
#if DEBUG
        builder.Logging.AddConsole();
#endif

        // JSON global case-insensitive
        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        // URL DA API
        var apiUrl = DeviceInfo.Current.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7042"
            : "https://192.168.15.15:7042";

#if !DEBUG
        apiUrl = "https://sua-api-railway.up.railway.app";
#endif

        // REFIT + JWT + RESILIÊNCIA (agora funciona!)
        builder.Services
            .AddRefitClient<IApiService>(new RefitSettings
            {
                AuthorizationHeaderValueGetter = async (msg, ct) =>
                    "Bearer " + (await SecureStorage.Default.GetAsync("jwt_token") ?? "")
            })
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(apiUrl);
                c.Timeout = TimeSpan.FromSeconds(60);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            })
            .AddStandardResilienceHandler(); // ← AGORA COMPILA!

        // SQLite local
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "financas.db3");
        builder.Services.AddSingleton<SQLiteAsyncConnection>(sp =>
        {
            var conn = new SQLiteAsyncConnection(dbPath);
            Task.Run(async () =>
            {
                await conn.CreateTableAsync<AccountLocal>();
                await conn.CreateTableAsync<TransactionLocal>();
                await conn.CreateTableAsync<CreditCardLocal>();
                await conn.CreateTableAsync<InvoiceLocal>();
                await conn.CreateTableAsync<TagLocal>();
                await conn.CreateTableAsync<TagAssignmentLocal>();
            }).GetAwaiter().GetResult();
            return conn;
        });

        // Repositórios + Serviços + ViewModels + Pages (mantidos iguais)
        builder.Services.AddScoped<InvoiceLocalRepository>();
        builder.Services.AddScoped<CreditCardLocalRepository>();
        builder.Services.AddScoped<TransactionLocalRepository>();
        builder.Services.AddScoped<TagLocalRepository>();
        builder.Services.AddScoped<TagAssignmentLocalRepository>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        builder.Services.AddSingleton<ILocalStorageService, SQLiteStorageService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<SyncService>();
        builder.Services.AddSingleton<ISyncService>(sp => sp.GetRequiredService<SyncService>());
        builder.Services.AddSingleton<InvoiceSyncService>();
        builder.Services.AddSingleton<ITransactionSyncService, TransactionSyncService>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<AccountsViewModel>();
        builder.Services.AddTransient<NewTransactionViewModel>();
        builder.Services.AddTransient<CreditCardsViewModel>();
        builder.Services.AddTransient<InvoiceDetailViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<AccountsPage>();
        builder.Services.AddTransient<NewTransactionPage>();
        builder.Services.AddTransient<CreditCardsPage>();
        builder.Services.AddTransient<InvoiceDetailPage>();
        builder.Services.AddTransient<ReportsPage>();

        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        return builder.Build();
    }
}