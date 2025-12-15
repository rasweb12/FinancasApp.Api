// Views/Dashboard/HomePage.xaml.cs
using FinancasApp.Mobile.ViewModels;
using Microsoft.Extensions.Logging;  // ◄ Adicione para ILogger

namespace FinancasApp.Mobile.Views.Dashboard;

public partial class HomePage : ContentPage
{
    private readonly ILogger<HomePage> _logger;

    public HomePage(ILogger<HomePage> logger)  // ◄ Injeção de ILogger
    {
        InitializeComponent();
        _logger = logger;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is HomeViewModel vm)
        {
            try
            {
                await vm.InitializeAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro no Dashboard",
                    $"Detalhes: {ex.Message}\nInner: {ex.InnerException?.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"[HomePage] Erro: {ex}");
            }
        }
    }
}