// ViewModels/CreditCardsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Storage;
using FinancasApp.Mobile.Services.Sync;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinancasApp.Mobile.ViewModels;

public partial class CreditCardsViewModel : ObservableObject
{
    private readonly ILocalStorageService _local;
    private readonly SyncService _sync;
    private readonly ILogger<CreditCardsViewModel> _logger;

    [ObservableProperty] private ObservableCollection<CreditCardLocal> cards = new();
    [ObservableProperty] private bool isBusy;

    public CreditCardsViewModel(
        ILocalStorageService local,
        SyncService sync,
        ILogger<CreditCardsViewModel> logger)
    {
        _local = local;
        _sync = sync;
        _logger = logger;
    }

    // -----------------------------------------------------
    // LOAD CARDS
    // -----------------------------------------------------
    [RelayCommand]
    private async Task LoadCardsAsync()
    {
        IsBusy = true;
        try
        {
            // CORRIGIDO: Usa o método correto da interface atual
            var list = await _local.GetAllAsync<CreditCardLocal>();

            Cards.Clear();
            foreach (var card in list)
                Cards.Add(card);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar cartões de crédito");
            await Shell.Current.DisplayAlert("Erro", "Não foi possível carregar os cartões.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // -----------------------------------------------------
    // ADD CARD
    // -----------------------------------------------------
    // ViewModels/CreditCardsViewModel.cs
    [RelayCommand]
    private async Task AddCardAsync()
    {
        try
        {
            var name = await Shell.Current.DisplayPromptAsync(
                "Novo Cartão",
                "Nome do cartão",
                "Adicionar",
                "Cancelar",
                "Ex: Nubank Gold");

            if (string.IsNullOrWhiteSpace(name)) return;

            var last4 = await Shell.Current.DisplayPromptAsync(
                "Últimos 4 dígitos",
                "Digite os 4 últimos números",
                "OK",
                "Cancelar",
                "1234");

            var limitStr = await Shell.Current.DisplayPromptAsync(
                "Limite",
                "Qual o limite total?",
                "OK",
                "Cancelar",
                "5000");

            var dueDayStr = await Shell.Current.DisplayPromptAsync(
                "Dia do vencimento",
                "Dia da fatura (1-31)",
                "OK",
                "Cancelar",
                "10");

            if (!decimal.TryParse(limitStr, out var limit)) limit = 0;
            if (!int.TryParse(dueDayStr, out var dueDay) || dueDay < 1 || dueDay > 31) dueDay = 10;

            var card = new CreditCardLocal
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Last4Digits = string.IsNullOrWhiteSpace(last4) ? "0000" : last4.PadLeft(4, '0')[..4],
                CreditLimit = limit,
                DueDay = dueDay,
                ClosingDay = Math.Max(1, dueDay - 10),
                IsDirty = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _local.SaveAsync(card);  // ← Método existe na interface
            await LoadCardsAsync();
            await _sync.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar cartão");
            await Shell.Current.DisplayAlert("Erro", "Não foi possível salvar o cartão.", "OK");
        }
    }

    // -----------------------------------------------------
    // VIEW INVOICE
    // -----------------------------------------------------
    [RelayCommand]
    private async Task ViewInvoiceAsync(CreditCardLocal card)
    {
        if (card?.Id == null) return;
        await AppShell.NavigateToInvoiceAsync(card.Id);
    }
}