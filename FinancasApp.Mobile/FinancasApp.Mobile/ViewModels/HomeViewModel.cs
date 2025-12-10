// ViewModels/HomeViewModel.cs — VERSÃO FINAL E IMORTAL
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Storage;
using FinancasApp.Mobile.Services.Sync;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace FinancasApp.Mobile.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly ILocalStorageService _local;
    private readonly ISyncService _sync; // ← CORRIGIDO: usa a interface
    private readonly ILogger<HomeViewModel> _logger;

    [ObservableProperty] private decimal totalBalance;
    [ObservableProperty] private ObservableCollection<TransactionLocal> recentTransactions = new();
    [ObservableProperty] private ISeries[] balanceSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] xAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] yAxes = Array.Empty<Axis>();
    [ObservableProperty] private string statusMessage = "Carregando...";
    [ObservableProperty] private bool isBusy;

    public IAsyncRelayCommand RefreshCommand { get; }

    public HomeViewModel(
        ILocalStorageService local,
        ISyncService sync, // ← INJEÇÃO CORRETA
        ILogger<HomeViewModel> logger)
    {
        _local = local;
        _sync = sync;
        _logger = logger;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
    }

    public async Task InitializeAsync()
    {
        try
        {
            LoadChartPlaceholder();
            await LoadFromCacheAsync();
            await RefreshCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            // NUNCA DEIXA O DASHBOARD QUEBRAR
            _logger.LogError(ex, "Erro crítico no InitializeAsync do Dashboard");
            StatusMessage = "Erro ao carregar. Puxe para atualizar.";
        }
    }

    private async Task LoadFromCacheAsync()
    {
        try
        {
            var accounts = await _local.GetAccountsAsync() ?? new List<AccountLocal>();
            TotalBalance = accounts.Sum(a => a.Balance);

            var transactions = await _local.GetTransactionsAsync() ?? new List<TransactionLocal>();
            RecentTransactions.Clear();
            foreach (var t in transactions.OrderByDescending(t => t.Date).Take(5))
                RecentTransactions.Add(t);

            StatusMessage = accounts.Any()
                ? $"Cache • {accounts.Count} conta(s)"
                : "Nenhuma conta cadastrada";
        }
        catch (Exception ex)
        {
            StatusMessage = "Erro ao carregar dados locais";
            _logger.LogError(ex, "Falha ao carregar cache");
        }
    }

    private void LoadChartPlaceholder()
    {
        BalanceSeries = new ISeries[]
        {
            new LineSeries<decimal>
            {
                Values = [0m, 0m, 0m, 0m, 0m, 0m],
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.LightGray.WithAlpha(80), 2),
                GeometrySize = 0,
                Name = "Saldo"
            }
        };

        XAxes = new[] { new Axis { Labels = ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun"] } };
        YAxes = new[] { new Axis { MinLimit = 0 } };
    }

    private async Task RefreshAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = "Sincronizando...";

        try
        {
            await _sync.SyncAllAsync();
            await LoadFromCacheAsync();
            await LoadChartAsync();
            StatusMessage = $"Atualizado • {DateTime.Now:HH:mm}";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sincronização falhou — usando dados locais");
            await LoadFromCacheAsync(); // Garante que pelo menos o cache apareça
            StatusMessage = "Offline • dados locais";
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
            var transactions = await _local.GetTransactionsAsync() ?? new List<TransactionLocal>();
            var valid = transactions
                .Where(t => t.Date != default)
                .OrderBy(t => t.Date)
                .ToList();

            if (!valid.Any())
            {
                LoadChartPlaceholder();
                return;
            }

            var grouped = valid
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .TakeLast(6)
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM/yy"),
                    Balance = g.Sum(t => t.Type == "Income" ? t.Amount : -t.Amount)
                })
                .ToList();

            if (!grouped.Any())
            {
                LoadChartPlaceholder();
                return;
            }

            decimal acumulado = 0;
            var values = grouped.Select(g => acumulado += g.Balance).ToArray();

            BalanceSeries = new ISeries[]
            {
                new LineSeries<decimal>
                {
                    Values = values,
                    Fill = new SolidColorPaint(SKColors.DeepSkyBlue.WithAlpha(40)),
                    Stroke = new SolidColorPaint(SKColors.DeepSkyBlue, 4),
                    GeometrySize = 12,
                    GeometryStroke = new SolidColorPaint(SKColors.White, 3),
                    GeometryFill = new SolidColorPaint(SKColors.DeepSkyBlue),
                    Name = "Saldo Acumulado"
                }
            };

            XAxes = new[] { new Axis { Labels = grouped.Select(x => x.Month).ToArray() } };
            YAxes = new[] { new Axis { Labeler = value => $"R$ {value:N0}", MinLimit = 0 } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar gráfico");
            LoadChartPlaceholder();
            StatusMessage = "Gráfico indisponível";
        }
    }
}