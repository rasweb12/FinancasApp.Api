// MauiProgram.cs — VERSÃO FINAL, IMORTAL E SEM ERRO (.NET 9 | Android 15)
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

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        // 🌍 DEFINIÇÃO DE AMBIENTE (CORRETA PARA ANDROID)
#if DEBUG
        var apiUrl = DeviceInfo.Current.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:7042" // HTTP para Android em DEV
            : "https://localhost:7042"; // HTTPS em outros dispositivos
#else
    var apiUrl = "https://financasapp-api.up.railway.app"; // PROD
#endif

        // 🔐 REFIT + JWT
        builder.Services
            .AddRefitClient<IApiService>(new RefitSettings
            {
                AuthorizationHeaderValueGetter = async (request, _) =>
                {
                    var path = request.RequestUri?.AbsolutePath ?? "";

                    if (path.Contains("/auth/login", StringComparison.OrdinalIgnoreCase) ||
                        path.Contains("/auth/register", StringComparison.OrdinalIgnoreCase))
                        return null;

                    var token = await SecureStorage.Default.GetAsync("jwt_token");
                    return string.IsNullOrWhiteSpace(token) ? null : $"Bearer {token}";
                }
            })
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(apiUrl);
                c.Timeout = TimeSpan.FromMinutes(5);
            })
            .AddStandardResilienceHandler(options =>
            {
                options.AttemptTimeout!.Timeout = TimeSpan.FromSeconds(120);
                options.TotalRequestTimeout!.Timeout = TimeSpan.FromMinutes(6);
                options.CircuitBreaker!.SamplingDuration = TimeSpan.FromMinutes(10);
                options.Retry!.MaxRetryAttempts = 3;
                options.Retry!.BackoffType = Polly.DelayBackoffType.Exponential;
            });

        // 💾 SQLite local
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "financas.db3");
        builder.Services.AddSingleton(_ =>
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

        // 📦 Repositórios e serviços
        builder.Services.AddScoped<InvoiceLocalRepository>();
        builder.Services.AddScoped<CreditCardLocalRepository>();
        builder.Services.AddScoped<TransactionLocalRepository>();
        builder.Services.AddScoped<TagLocalRepository>();
        builder.Services.AddScoped<TagAssignmentLocalRepository>();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        builder.Services.AddSingleton<ILocalStorageService, SQLiteStorageService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ISyncService, SyncService>();

        // 🧠 ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<AccountsViewModel>();
        builder.Services.AddTransient<NewTransactionViewModel>();
        builder.Services.AddTransient<CreditCardsViewModel>();
        builder.Services.AddTransient<InvoiceDetailViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();

        // 📱 Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<AccountsPage>();
        builder.Services.AddTransient<NewTransactionPage>();
        builder.Services.AddTransient<CreditCardsPage>();
        builder.Services.AddTransient<InvoiceDetailPage>();
        builder.Services.AddTransient<ReportsPage>();

        // 🧭 Shell
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}
