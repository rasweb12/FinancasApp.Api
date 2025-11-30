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
            var list = await _local.GetAllCreditCardsAsync();

            Cards.Clear();
            foreach (var c in list)
                Cards.Add(c);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar cartões");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // -----------------------------------------------------
    // ADD CARD
    // -----------------------------------------------------
    [RelayCommand]
    private async Task AddCardAsync()
    {
        try
        {
            var name = await Application.Current!.MainPage!.DisplayPromptAsync(
                "Novo Cartão", "Nome do cartão:");

            if (string.IsNullOrWhiteSpace(name)) return;

            var last4 = await Application.Current!.MainPage!.DisplayPromptAsync(
                "Últimos 4 dígitos", "Ex: 1234");

            var limitStr = await Application.Current!.MainPage!.DisplayPromptAsync(
                "Limite", "R$ 0,00");

            var dueDayStr = await Application.Current!.MainPage!.DisplayPromptAsync(
                "Dia do vencimento", "1-31");

            decimal.TryParse(limitStr, out var limit);
            int.TryParse(dueDayStr, out var dueDay);

            if (dueDay < 1 || dueDay > 31) dueDay = 10;

            var card = new CreditCardLocal
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                Last4Digits = last4 ?? "0000",
                CreditLimit = limit,
                DueDay = dueDay,
                ClosingDay = Math.Max(1, dueDay - 10),
                CurrentInvoiceId = null,
                //IsNew = true,
                IsDirty = true
            };

            await _local.SaveCreditCardAsync(card);

            await LoadCardsAsync();
            await _sync.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar novo cartão");
        }
    }

    // -----------------------------------------------------
    // VIEW INVOICE
    // -----------------------------------------------------
    [RelayCommand]
    private async Task ViewInvoiceAsync(CreditCardLocal card)
    {
        await AppShell.NavigateToInvoiceAsync(card.Id);
    }
}
