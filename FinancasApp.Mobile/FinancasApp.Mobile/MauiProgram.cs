// MauiProgram.cs — VERSÃO FINAL, IMORTAL E SEM ERRO (.NET 9 – 06/12/2025)
using System.Text.Json;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.Auth;
using FinancasApp.Mobile.Services.LocalDatabase;
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

        LiveCharts.Configure(config => config
            .AddSkiaSharp()
            .AddDefaultMappers()
            .AddLightTheme());

#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.AddConsole();
#endif

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        var apiUrl = DeviceInfo.Current.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7042"
            : "https://localhost:7042";

#if !DEBUG
        apiUrl = "https://sua-api-railway.up.railway.app";
#endif

        // REFIT + JWT INTELIGENTE (CORRIGIDO O PARÂMETRO!)
        builder.Services
            .AddRefitClient<IApiService>(new RefitSettings
            {
                AuthorizationHeaderValueGetter = async (httpRequestMessage, cancellationToken) =>
                {
                    // CORREÇÃO: o parâmetro é HttpRequestMessage, não "msg"
                    var path = httpRequestMessage.RequestUri?.AbsolutePath ?? "";

                    if (path.Contains("/auth/login", StringComparison.OrdinalIgnoreCase) ||
                        path.Contains("/auth/register", StringComparison.OrdinalIgnoreCase))
                    {
                        return null; // ← NÃO ENVIA TOKEN NO LOGIN/REGISTER
                    }

                    var token = await SecureStorage.Default.GetAsync("jwt_token");
                    return string.IsNullOrEmpty(token) ? null : $"Bearer {token}";
                }
            })
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(apiUrl);
                c.Timeout = TimeSpan.FromMinutes(5);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (handler.SupportsAutomaticDecompression)
                    handler.AutomaticDecompression = System.Net.DecompressionMethods.All;

                if (apiUrl.Contains("10.0.2.2") || apiUrl.Contains("localhost"))
                {
                    handler.ServerCertificateCustomValidationCallback =
                        (message, cert, chain, errors) => true;
                }
                return handler;
            })
            .AddStandardResilienceHandler(options =>
            {
                options.AttemptTimeout!.Timeout = TimeSpan.FromSeconds(120);
                options.TotalRequestTimeout!.Timeout = TimeSpan.FromMinutes(6);
                options.CircuitBreaker!.SamplingDuration = TimeSpan.FromMinutes(10);
                options.Retry!.MaxRetryAttempts = 3;
                options.Retry!.BackoffType = Polly.DelayBackoffType.Exponential;
            });

        // Banco SQLite local
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

        // Repositórios e Serviços
        builder.Services.AddScoped<InvoiceLocalRepository>();
        builder.Services.AddScoped<CreditCardLocalRepository>();
        builder.Services.AddScoped<TransactionLocalRepository>();
        builder.Services.AddScoped<TagLocalRepository>();
        builder.Services.AddScoped<TagAssignmentLocalRepository>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        builder.Services.AddSingleton<ILocalStorageService, SQLiteStorageService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ISyncService, SyncService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<AccountsViewModel>();
        builder.Services.AddTransient<NewTransactionViewModel>();
        builder.Services.AddTransient<CreditCardsViewModel>();
        builder.Services.AddTransient<InvoiceDetailViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>(provider =>
        {
            var page = new HomePage();
            page.BindingContext = provider.GetRequiredService<HomeViewModel>();
            return page;
        });
        builder.Services.AddTransient<AccountsPage>();
        builder.Services.AddTransient<NewTransactionPage>();
        builder.Services.AddTransient<CreditCardsPage>();
        builder.Services.AddTransient<InvoiceDetailPage>();
        builder.Services.AddTransient<ReportsPage>();
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}