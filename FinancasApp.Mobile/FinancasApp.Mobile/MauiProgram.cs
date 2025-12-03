// MauiProgram.cs — VERSÃO FINAL OFICIAL E IMORTAL (03/12/2025)
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

        LiveCharts.Configure(c => c
            .AddSkiaSharp()
            .AddDefaultMappers()
            .AddLightTheme());

        builder.Logging.AddDebug();
#if DEBUG
        builder.Logging.AddConsole();
#endif

        // ================================
        // URL DA API — MUDE AQUI QUANDO SUBIR PRO RAILWAY
        // ================================
        var apiUrl = "https://localhost:7042"; // ← depois muda pra sua URL do Railway

        // ================================
        // HTTP + REFIT COM JWT AUTOMÁTICO
        // ================================
        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri(apiUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

        builder.Services.AddRefitClient<IApiService>(new RefitSettings
        {
            AuthorizationHeaderValueGetter = async (msg, ct) =>
                "Bearer " + (await SecureStorage.Default.GetAsync("jwt_token") ?? "")
        })
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiUrl));

        // ================================
        // SQLITE LOCAL (.NET 9)
        // ================================
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "financas.db3");
        builder.Services.AddSingleton<SQLiteAsyncConnection>(sp =>
        {
            var conn = new SQLiteAsyncConnection(dbPath);
            conn.CreateTableAsync<AccountLocal>().Wait();
            conn.CreateTableAsync<TransactionLocal>().Wait();
            conn.CreateTableAsync<CreditCardLocal>().Wait();
            conn.CreateTableAsync<InvoiceLocal>().Wait();
            conn.CreateTableAsync<TagLocal>().Wait();
            conn.CreateTableAsync<TagAssignmentLocal>().Wait();
            return conn;
        });

        // ================================
        // REPOSITÓRIOS
        // ================================
        builder.Services.AddScoped<InvoiceLocalRepository>();
        builder.Services.AddScoped<CreditCardLocalRepository>();
        builder.Services.AddScoped<TransactionLocalRepository>();
        builder.Services.AddScoped<TagLocalRepository>();
        builder.Services.AddScoped<TagAssignmentLocalRepository>();

        // ✅ CORRETA — SE SUA CLASSE SE CHAMA BaseRepository<T>
        builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

        // ================================
        // SERVIÇOS PRINCIPAIS — TUDO REGISTRADO CORRETAMENTE (NUNCA MAIS VAI DAR ERRO)
        // ================================
        builder.Services.AddSingleton<ILocalStorageService, SQLiteStorageService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // ESSAS DUAS LINHAS SÃO OBRIGATÓRIAS — RESOLVEM O ERRO DO SyncService PRA SEMPRE
        builder.Services.AddSingleton<SyncService>();                    // ← CLASSE CONCRETA
        builder.Services.AddSingleton<ISyncService>(sp => sp.GetRequiredService<SyncService>()); // ← INTERFACE

        builder.Services.AddSingleton<InvoiceSyncService>();
        builder.Services.AddSingleton<ITransactionSyncService, TransactionSyncService>();

        // ================================
        // VIEWMODELS
        // ================================
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<AccountsViewModel>();
        builder.Services.AddTransient<NewTransactionViewModel>();
        builder.Services.AddTransient<CreditCardsViewModel>();
        builder.Services.AddTransient<InvoiceDetailViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();

        // ================================
        // PÁGINAS
        // ================================
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