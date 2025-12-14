// App.xaml.cs — VERSÃO FINAL, SEM NENHUM ERRO DE DI
using FinancasApp.Mobile.Services.Sync;
using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile;

public partial class App : Application
{
    private readonly ISyncService _syncService;
    private readonly ILogger<App> _logger;
    private readonly AppShell _appShell;

    public App(ISyncService syncService, ILogger<App> logger, AppShell appShell)
    {
        _syncService = syncService;
        _logger = logger;
        _appShell = appShell;

        InitializeComponent();
        MainPage = _appShell; // ← O DI já injetou o ViewModel no AppShell

        HandleGlobalExceptions();
    }

    protected override void OnStart()
    {
        base.OnStart();
        _ = TrySyncAsync();
    }

    private async Task TrySyncAsync()
    {
        try
        {
            await _syncService.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sincronização inicial falhou — continuando offline");
        }
    }

    private void HandleGlobalExceptions()
    {
        AppDomain.CurrentDomain.UnhandledException += async (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            _logger.LogCritical(ex, "Exceção global não tratada");

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Current?.MainPage != null)
                {
                    await Current.MainPage.DisplayAlert(
                        "Erro Crítico",
                        "Ocorreu um erro inesperado.",
                        "OK");
                }
            });
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            _logger.LogCritical(args.Exception, "Exceção não observada em task");
            args.SetObserved();
        };
    }
}