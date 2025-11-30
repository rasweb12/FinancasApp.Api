using FinancasApp.Mobile.Models.Local;
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
            .UseSkiaSharp(true) // Ativa SkiaSharp globalmente (LiveCharts, etc.)
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

        // LiveChartsCore + Tema Claro
        LiveCharts.Configure(config =>
            config
                .AddSkiaSharp()
                .AddDefaultMappers()
                .AddLightTheme());

        // Logging (Debug + Console em desenvolvimento)
        builder.Logging.AddDebug();
#if DEBUG
        builder.Logging.AddConsole();
#endif

        // ========================================
        // HttpClient + Refit (com JWT automático)
        // ========================================
        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7001"); // Mude para produção depois
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddRefitClient<IApiService>(new RefitSettings
        {
            AuthorizationHeaderValueGetter = async (msg, cancellationToken) =>
            {
                var token = await SecureStorage.Default.GetAsync("jwt_token");
                return string.IsNullOrEmpty(token) ? string.Empty : token;
            }
        })
        .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7001"))
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Aceita certificado dev (remover em prod)
        });

        // ========================================
        // SQLite + Banco de Dados Local
        // ========================================
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "financas.db3");

        builder.Services.AddSingleton<SQLiteAsyncConnection>(sp =>
        {
            var connection = new SQLiteAsyncConnection(dbPath);

            // Criação automática das tabelas (com await seguro)
            Task.Run(async () =>
            {
                await connection.CreateTableAsync<AccountLocal>();
                await connection.CreateTableAsync<TransactionLocal>();
                await connection.CreateTableAsync<CreditCardLocal>();
                await connection.CreateTableAsync<InvoiceLocal>();
            }).GetAwaiter().GetResult();

            return connection;
        });

        // ========================================
        // Repositórios Locais (BaseRepository + Específicos)
        // ========================================
        builder.Services.AddScoped<IBaseRepository<AccountLocal>, AccountRepository>();
        builder.Services.AddScoped<IBaseRepository<TransactionLocal>, TransactionRepository>();
        builder.Services.AddScoped<IBaseRepository<CreditCardLocal>, CreditCardRepository>();
        builder.Services.AddScoped<IBaseRepository<InvoiceLocal>, InvoiceRepository>();

        // ========================================
        // Serviços Principais
        // ========================================
        builder.Services.AddSingleton<ILocalStorageService, SQLiteStorageService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // Sync Services
        builder.Services.AddSingleton<ISyncService, SyncService>();
        builder.Services.AddSingleton<IInvoiceSyncService, InvoiceSyncService>();
        builder.Services.AddSingleton<ITransactionSyncService, TransactionSyncService>();

        // ========================================
        // ViewModels (Transient = nova instância por página)
        // ========================================
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<AccountsViewModel>();
        builder.Services.AddTransient<NewTransactionViewModel>();
        builder.Services.AddTransient<CreditCardsViewModel>();
        builder.Services.AddTransient<InvoiceDetailViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();

        // ========================================
        // Views (Páginas)
        // ========================================
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<AccountsPage>();
        builder.Services.AddTransient<NewTransactionPage>();
        builder.Services.AddTransient<CreditCardsPage>();
        builder.Services.AddTransient<InvoiceDetailPage>();
        builder.Services.AddTransient<ReportsPage>();

        // Shell (Singleton)
        builder.Services.AddSingleton<AppShell>();

        // App principal
        builder.Services.AddSingleton<App>();

        // ========================================
        // Finalização
        // ========================================
        return builder.Build();
    }
}