using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Services.Storage;
using FinancasApp.Mobile.Services.Sync;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace FinancasApp.Mobile.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IApiService _api;
    private readonly ILocalStorageService _local;
    private readonly SyncService _sync;
    private readonly ILogger<HomeViewModel> _logger;

    // Bindable properties
    [ObservableProperty] private decimal totalBalance;
    [ObservableProperty] private ObservableCollection<TransactionDto> recentTransactions = new();
    [ObservableProperty] private ISeries[] balanceSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] xAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] yAxes = Array.Empty<Axis>();

    // 👉 Comando público correto
    public IAsyncRelayCommand RefreshCommand { get; }

    public HomeViewModel(
        IApiService api,
        ILocalStorageService local,
        SyncService sync,
        ILogger<HomeViewModel> logger)
    {
        _api = api;
        _local = local;
        _sync = sync;
        _logger = logger;

        Title = "Dashboard";

        // 👉 Inicializa o comando aqui (correto)
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
    }

    // Inicialização
    public async Task InitializeAsync()
    {
        try
        {
            LoadChart();
            await LoadFromCacheAsync();

            // 👉 Executa o comando normalmente
            await RefreshCommand.ExecuteAsync(null);

            await LoadChartAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar HomeViewModel");
            StatusMessage = "Erro ao carregar dados";
        }
    }

    private async Task LoadFromCacheAsync()
    {
        try
        {
            var accounts = await _local.GetAccountsAsync();
            TotalBalance = accounts.Sum(a => a.Balance);
            StatusMessage = accounts.Any()
                ? $"Atualizado localmente ({accounts.Count()} contas)"
                : "Nenhuma conta encontrada";
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro ao carregar cache";
            _logger.LogError(ex, "Falha ao carregar dados locais");
        }
    }

    private void LoadChart()
    {
        BalanceSeries = new ISeries[]
        {
            new LineSeries<decimal>
            {
                Values = new decimal[] { 2500, 3200, 2800, 3500, 4000, 4500 },
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.DeepSkyBlue, 4),
                GeometryFill = null,
                GeometryStroke = null
            }
        };

        XAxes = new Axis[] { new() { Labels = new[] { "Jan", "Fev", "Mar", "Abr", "Mai", "Jun" } } };
        YAxes = new Axis[] { new() { MinLimit = 0 } };
    }

    private async Task RefreshAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = "Sincronizando dados...";

        try
        {
            await _sync.SyncAllAsync();
            await LoadFromCacheAsync();

            var transactions = await _api.GetTransactionsAsync();
            RecentTransactions.Clear();
            foreach (var t in transactions.Take(5))
                RecentTransactions.Add(t);

            StatusMessage = $"Atualizado às {DateTime.Now:HH:mm}";
            await LoadChartAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao sincronizar");
            StatusMessage = "Modo offline (cache)";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadChartAsync()
    {
        try
        {
            var transactions = await _local.GetTransactionsAsync();
            var valid = transactions.Where(t => t.Date != default).ToList();

            var grouped = valid
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                    Balance = g.Sum(t => t.Amount)
                })
                .ToList();

            if (!grouped.Any())
            {
                grouped = Enumerable.Range(1, 6)
                    .Select(i => new { Month = $"M{i}", Balance = 0m })
                    .ToList();
            }

            decimal acumulado = 0;
            var values = grouped.Select(g => acumulado += g.Balance).ToArray();

            BalanceSeries = new ISeries[]
            {
                new LineSeries<decimal>
                {
                    Values = values,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.DeepSkyBlue, 4),
                    GeometryStroke = null,
                    GeometryFill = null
                }
            };

            XAxes = new Axis[] { new() { Labels = grouped.Select(x => x.Month).ToArray() } };
            YAxes = new Axis[] { new() { MinLimit = 0 } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao montar gráfico");
            StatusMessage = "Falha ao gerar gráfico";
        }
    }
}
