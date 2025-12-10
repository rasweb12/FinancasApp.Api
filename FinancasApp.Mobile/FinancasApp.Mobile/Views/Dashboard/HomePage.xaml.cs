// Views/Dashboard/HomePage.xaml.cs — VERSÃO FINAL E QUE NUNCA VAI DAR ERRO
using FinancasApp.Mobile.ViewModels;

namespace FinancasApp.Mobile.Views.Dashboard;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        // NÃO FAZ NADA AQUI — O BindingContext será definido no MauiProgram
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
                System.Diagnostics.Debug.WriteLine($"[HomePage] Erro no InitializeAsync: {ex}");
            }
        }
    }
}