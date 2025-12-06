// App.xaml.cs — VERSÃO FINAL, LIMPA E QUE NUNCA TRAVA
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

        // Sync só quando o usuário quiser (NUNCA na inicialização)
        // _ = InicializarSincronizacaoAsync(); ← APAGUE ESSA LINHA
    }

    private void RegistrarTratamentoDeExcecoesGlobais()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            _logger.LogCritical(e.ExceptionObject as Exception, "EXCEÇÃO NÃO TRATADA");

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            _logger.LogCritical(e.Exception, "EXCEÇÃO NÃO OBSERVADA");
            e.SetObserved();
        };
    }

    // Sync manual — chame isso só com botão ou pull-to-refresh
    public async Task TrySyncAsync()
    {
        try
        {
            await _syncService.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sync falhou — modo offline ativo");
            // Não trava o app, só fica offline
        }
    }
}