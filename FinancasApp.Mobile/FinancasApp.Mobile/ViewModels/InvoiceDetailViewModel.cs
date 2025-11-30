// ViewModels/InvoiceDetailViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Storage;
using FinancasApp.Mobile.Services.Sync;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinancasApp.Mobile.ViewModels;

public partial class InvoiceDetailViewModel : ObservableObject
{
    private readonly ILocalStorageService _local;
    private readonly SyncService _sync;
    private readonly ILogger<InvoiceDetailViewModel> _logger;
    private Guid _cardId;

    [ObservableProperty] private CreditCardLocal? card;
    [ObservableProperty] private InvoiceLocal? invoice;
    [ObservableProperty] private ObservableCollection<InstallmentItem> installments = new();
    [ObservableProperty] private decimal partialPayment;

    public string CardName => Card?.Name ?? "Cartão";

    public InvoiceDetailViewModel(
        ILocalStorageService local,
        SyncService sync,
        ILogger<InvoiceDetailViewModel> logger)
    {
        _local = local;
        _sync = sync;
        _logger = logger;
    }

    public async Task InitializeAsync(Guid cardId)
    {
        _cardId = cardId;

        // Corrigido: usando _local (não _storage)
        Card = await _local.GetCreditCardByIdAsync(cardId);
        Invoice = await _local.GetCurrentInvoiceAsync();

        LoadInstallments();
        OnPropertyChanged(nameof(CardName));
    }

    private void LoadInstallments()
    {
        Installments.Clear();
        Installments.Add(new InstallmentItem { Description = "Netflix", Amount = 39.90m, InstallmentNumber = "1/1" });
        Installments.Add(new InstallmentItem { Description = "Uber", Amount = 45.00m, InstallmentNumber = "1/3" });
        Installments.Add(new InstallmentItem { Description = "iFood", Amount = 78.50m, InstallmentNumber = "2/3" });
    }

    [RelayCommand]
    private async Task PayPartialAsync()
    {
        if (PartialPayment <= 0 || Invoice is null) return;

        Invoice.PaidAmount += PartialPayment;
        Invoice.IsPaid = Invoice.PaidAmount >= Invoice.Total;
        Invoice.IsDirty = true; // Agora usa IsDirty corretamente

        await _local.SaveInvoiceAsync(Invoice); // Usa _local
        await _sync.SyncAllAsync();

        PartialPayment = 0;
        OnPropertyChanged(nameof(Invoice));
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// Método genérico para atualizar qualquer campo da fatura
    /// </summary>
    public async Task UpdateInvoice(Action<InvoiceLocal> update)
    {
        if (Invoice is null) return;

        update(Invoice);
        Invoice.IsDirty = true;

        await _local.SaveInvoiceAsync(Invoice);
    }
}

public class InstallmentItem
{
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public string InstallmentNumber { get; set; } = "";
}