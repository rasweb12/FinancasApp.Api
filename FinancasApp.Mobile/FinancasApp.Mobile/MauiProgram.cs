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
using FinancasApp.Mobile.Views.Categories;
using FinancasApp.Mobile.Views.CreditCards;
using FinancasApp.Mobile.Views.Dashboard;
using FinancasApp.Mobile.Views.Reports;
using FinancasApp.Mobile.Views.Transactions;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using Refit;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SQLite;
using System.Text.Json;

namespace FinancasApp.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseLiveCharts()
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
            options.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

#if DEBUG
        var apiUrl = DeviceInfo.Current.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7043"
            : "https://localhost:7043";
#else
        var apiUrl = "https://financasapp-api.up.railway.app";
#endif

#if DEBUG
        builder.Services.AddSingleton<HttpClientHandler>(sp =>
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                (message, cert, chain, errors) => true;
            return handler;
        });
#endif

        // 🔐 Refit + JWT (sem log temporário para evitar erros)
        builder.Services
            .AddRefitClient<IApiService>(new RefitSettings
            {
                AuthorizationHeaderValueGetter = async (request, _) =>
                {
                    var path = request.RequestUri?.AbsolutePath ?? "";
                    if (path.Contains("/auth/login") || path.Contains("/auth/register"))
                        return null;

                    var token = await SecureStorage.Default.GetAsync("jwt_token");
                    return string.IsNullOrWhiteSpace(token) ? null : $"Bearer {token}";
                }
            })
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(apiUrl);
                c.Timeout = TimeSpan.FromSeconds(300);
            })
#if DEBUG
            .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<HttpClientHandler>());
#endif

        // 💾 SQLite
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
            conn.CreateTableAsync<CategoryLocal>().Wait();
            return conn;
        });

        // ✅ Repositório Genérico
        builder.Services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

        // 📦 Serviços Mobile
        builder.Services.AddSingleton<ILocalStorageService, SQLiteStorageService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ISyncService, SyncService>();

        // Repositório específico para Category
        builder.Services.AddSingleton<IRepository<CategoryLocal>, Repository<CategoryLocal>>();

        // 🧠 ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<AccountsViewModel>();
        builder.Services.AddTransient<NewTransactionViewModel>();
        builder.Services.AddTransient<CreditCardsViewModel>();
        builder.Services.AddTransient<InvoiceDetailViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();
        builder.Services.AddTransient<CategoriesViewModel>();

        // 📱 Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<AccountsPage>();
        builder.Services.AddTransient<NewTransactionPage>();
        builder.Services.AddTransient<CreditCardsPage>();
        builder.Services.AddTransient<InvoiceDetailPage>();
        builder.Services.AddTransient<ReportsPage>();
        builder.Services.AddTransient<CategoriesPage>();

        // 🧭 Shell
        builder.Services.AddSingleton<AppShellViewModel>();
        builder.Services.AddSingleton<AppShell>();

        builder.Logging.AddDebug();

        return builder.Build();
    }
}