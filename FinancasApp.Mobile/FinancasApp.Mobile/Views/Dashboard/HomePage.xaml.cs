// Views/Dashboard/HomePage.xaml.cs — COM LAZY LOADING (Melhoria Aplicada)
using FinancasApp.Mobile.ViewModels;

namespace FinancasApp.Mobile.Views.Dashboard;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HomeViewModel vm)
        {
            try
            {
                // Lazy Loading: Carrega dados e gráfico após a página aparecer
                await vm.InitializeAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HomePage] Erro no InitializeAsync: {ex.Message}");
            }
        }
    }
}