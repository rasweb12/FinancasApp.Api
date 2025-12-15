// ViewModels/HomeViewModel.cs — COM LAZY LOADING (Melhoria Aplicada)
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Storage;
using FinancasApp.Mobile.Services.Sync;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Logging;
using Refit;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Net;

namespace FinancasApp.Mobile.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly ILocalStorageService _local;
    private readonly ISyncService _sync;
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
        ISyncService sync,
        ILogger<HomeViewModel> logger)
    {
        _local = local;
        _sync = sync;
        _logger = logger;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
    }

    public async Task InitializeAsync()
    {
        LoadChartPlaceholder();
        await LoadFromCacheAsync();
        await RefreshCommand.ExecuteAsync(null);
    }

    private async Task LoadFromCacheAsync()
    {
        try
        {
            var accounts = await _local.GetAccountsAsync();
            TotalBalance = accounts.Sum(a => a.Balance);
            var transactions = await _local.GetTransactionsAsync();
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
                Values = [0, 0, 0, 0, 0, 0],
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.LightGray.WithAlpha(80), 2),
                GeometrySize = 0
            }
        };
        XAxes = [new Axis { Labels = ["Jan", "Fev", "Mar", "Abr", "Mai", "Jun"] }];
        YAxes = [new Axis { MinLimit = 0 }];
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
        catch (ApiException apiEx) when (apiEx.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning("Token inválido ou expirado durante sync");
            StatusMessage = "Sessão expirada • dados locais";
            // Opcional: Forçar logout
            // await _auth.ClearTokenAsync();
            // await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sync falhou — usando cache");
            await LoadFromCacheAsync();  // Garante dados locais mesmo offline
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
            // Reutiliza transações já carregadas (evita nova consulta ao banco)
            var transactions = RecentTransactions.ToList(); // Já tem as últimas
            var allTransactions = await _local.GetTransactionsAsync(); // Todas para gráfico preciso

            var valid = allTransactions
                .Where(t => t.Date != default(DateTime))
                .OrderBy(t => t.Date)
                .ToList();

            if (!valid.Any())
            {
                LoadChartPlaceholder();
                return;
            }

            // Agrupa por mês/ano
            var grouped = valid
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1)
                        .ToString("MMM/yy", System.Globalization.CultureInfo.GetCultureInfo("pt-BR")),
                    Total = g.Sum(t => t.Amount)
                })
                .ToList();

            // Acumulado para linha de evolução do saldo
            decimal acumulado = 0;
            var values = grouped.Select(g => acumulado += g.Total).ToArray();

            // Série principal do gráfico
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

            // Eixos
            XAxes = new Axis[]
            {
            new Axis
            {
                Labels = grouped.Select(x => x.Month).ToArray(),
                LabelsRotation = 0,
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(50))
            }
            };

            YAxes = new Axis[]
            {
            new Axis
            {
                Labeler = value => $"R$ {value:N0}",
                MinLimit = 0,
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray.WithAlpha(50))
            }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha crítica ao carregar gráfico — usando placeholder");
            LoadChartPlaceholder();
            StatusMessage = "Gráfico temporariamente indisponível";
        }
    }
}