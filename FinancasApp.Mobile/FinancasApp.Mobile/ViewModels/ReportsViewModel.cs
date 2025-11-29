//ViewModels/ReportsViewModel.cs	
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Storage;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace FinancasApp.Mobile.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly ILocalStorageService _local;
    private readonly ILogger<ReportsViewModel> _logger;

    [ObservableProperty] private ISeries[] categorySeries = Array.Empty<ISeries>();
    [ObservableProperty] private ISeries[] dailySeries = Array.Empty<ISeries>();
    [ObservableProperty] private ISeries[] balanceSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] xAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] yAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] xAxesMonth = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] yAxesMoney = Array.Empty<Axis>();
    [ObservableProperty] private decimal monthlyTotal;

    public ReportsViewModel(ILocalStorageService local, ILogger<ReportsViewModel> logger)
    {
        _local = local;
        _logger = logger;
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            var transactions = await _local.GetTransactionsAsync();
            var now = DateTime.Today;

            // 1. PIZZA: Categorias
            var categoryGroups = transactions
                .Where(t => t.Amount < 0)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = Math.Abs(g.Sum(t => t.Amount)) })
                .OrderByDescending(x => x.Total)
                .Take(6);

            CategorySeries = categoryGroups.Select(g => new PieSeries<decimal>
            {
                Name = g.Category,
                Values = new[] { g.Total },
                Fill = new SolidColorPaint(RandomColor())
            }).Cast<ISeries>().ToArray();

            // 2. BARRAS: Últimos 7 dias
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => now.AddDays(-i))
                .Reverse();

            var dailyTotals = last7Days.Select(day =>
                transactions.Where(t => t.Date.Date == day.Date && t.Amount < 0)
                            .Sum(t => Math.Abs(t.Amount)))
                .ToArray();

            DailySeries = new ISeries[]
            {
                new ColumnSeries<decimal>
                {
                    Name = "Gastos",
                    Values = dailyTotals,
                    Fill = new SolidColorPaint(SKColors.Crimson)
                }
            };

            XAxes = new[] { new Axis { Labels = last7Days.Select(d => d.ToString("dd/MM")).ToArray() } };
            YAxes = new[] { new Axis { Labeler = value => value.ToString("C0") } };

            // 3. LINHA: Saldo mensal
            var monthlyBalance = new List<decimal> { 1000m };
            var dates = new List<string> { "01/11" };

            for (int day = 2; day <= now.Day; day++)
            {
                var daily = transactions
                    .Where(t => t.Date.Day == day && t.Date.Month == now.Month)
                    .Sum(t => t.Amount);
                monthlyBalance.Add(monthlyBalance.Last() + daily);
                dates.Add(day.ToString("00") + "/11");
            }

            BalanceSeries = new ISeries[]
            {
                new LineSeries<decimal>
                {
                    Values = monthlyBalance,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.RoyalBlue) { StrokeThickness = 3 }
                }
            };

            XAxesMonth = new[] { new Axis { Labels = dates.ToArray() } };
            YAxesMoney = new[] { new Axis { Labeler = value => value.ToString("C0") } };

            // Resumo
            MonthlyTotal = Math.Abs(transactions
                .Where(t => t.Date.Month == now.Month && t.Amount < 0)
                .Sum(t => t.Amount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar relatórios");
        }
    }

    private SKColor RandomColor()
    {
        var random = new Random();
        return new SKColor(
            (byte)random.Next(100, 255),
            (byte)random.Next(100, 255),
            (byte)random.Next(100, 255));
    }
}