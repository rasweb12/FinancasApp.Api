using FinancasApp.Mobile.Services.Sync;
using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile;

public partial class App : Application
{
    private readonly SyncService _syncService;
    private readonly ILogger<App> _logger;

    public App(SyncService syncService, ILogger<App> logger)
    {
        _logger = logger;
        _syncService = syncService;

        InitializeComponent();
        MainPage = new AppShell();

        RegistrarTratamentoDeExcecoesGlobais();

        // Iniciar sincronização inicial sem travar UI
        _ = InicializarSincronizacaoAsync();
    }

    private void RegistrarTratamentoDeExcecoesGlobais()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            _logger.LogCritical(ex, "EXCEÇÃO NÃO TRATADA (AppDomain) - App pode fechar.");
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            _logger.LogCritical(e.Exception, "EXCEÇÃO NÃO OBSERVADA (TaskScheduler)");
            e.SetObserved(); // evita crash em exceções pendentes
        };
    }

    private async Task InicializarSincronizacaoAsync()
    {
        try
        {
            await Task.Delay(1200); // dá tempo do app estabilizar

            _logger.LogInformation("Iniciando sincronização automática de inicialização...");

            await _syncService.SyncAllAsync();

            _logger.LogInformation("Sincronização automática concluída com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "FALHA CRÍTICA na sincronização automática ao iniciar o app.");
        }
    }
}
