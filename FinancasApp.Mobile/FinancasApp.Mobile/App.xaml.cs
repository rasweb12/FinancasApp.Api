// App.xaml.cs — VERSÃO FINAL, LIMPA E QUE NUNCA TRAVA
using FinancasApp.Mobile.Services.Sync;
using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile;

public partial class App : Application
{
    private readonly ISyncService _syncService;
    private readonly ILogger<App> _logger;

    public App(ISyncService syncService, ILogger<App> logger)
    {
        _syncService = syncService;
        _logger = logger;

        InitializeComponent();
        MainPage = new AppShell();

        RegistrarTratamentoDeExcecoesGlobais();
    }

    private void RegistrarTratamentoDeExcecoesGlobais()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            _logger.LogCritical(e.ExceptionObject as Exception, "EXCEÇÃO NÃO TRATADA NO APP");

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            _logger.LogCritical(e.Exception, "EXCEÇÃO NÃO OBSERVADA EM TASK");
            e.SetObserved();
        };
    }

    // Método público para ser chamado pelo Dashboard ou Pull-to-Refresh
    public async Task TrySyncAsync()
    {
        try
        {
            await _syncService.SyncAllAsync();
            _logger.LogInformation("Sincronização manual concluída com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sincronização falhou — continuando em modo offline");
            // O app NUNCA trava por causa disso
        }
    }
}