// ViewModels/AccountsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Storage;
using FinancasApp.Mobile.Services.Sync;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinancasApp.Mobile.ViewModels;

public partial class AccountsViewModel : ObservableObject
{
    private readonly ILocalStorageService _local;
    private readonly SyncService _sync;
    private readonly ILogger<AccountsViewModel> _logger;

    [ObservableProperty] private ObservableCollection<AccountLocal> accounts = new();
    [ObservableProperty] private decimal totalBalance;
    [ObservableProperty] private bool isBusy;

    public AccountsViewModel(
        ILocalStorageService local,
        SyncService sync,
        ILogger<AccountsViewModel> logger)
    {
        _local = local;
        _sync = sync;
        _logger = logger;
    }

    // -----------------------------------------------------
    // LOAD ACCOUNTS
    // -----------------------------------------------------
    [RelayCommand]
    private async Task LoadAccountsAsync()
    {
        IsBusy = true;
        try
        {
            // CORRIGIDO: Agora usa o método correto da interface
            var list = await _local.GetAccountsAsync();

            Accounts.Clear();
            foreach (var a in list)
                Accounts.Add(a);

            TotalBalance = list.Sum(a => a.Balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar contas");
            await Shell.Current.DisplayAlert("Erro", "Falha ao carregar contas", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // -----------------------------------------------------
    // ADD ACCOUNT
    // -----------------------------------------------------
    [RelayCommand]
    private async Task AddAccountAsync()
    {
        try
        {
            var name = await Shell.Current.DisplayPromptAsync(
                "Nova Conta", "Nome da conta:", placeholder: "Ex: Nubank, Caixa...");

            if (string.IsNullOrWhiteSpace(name)) return;

            var typeString = await Shell.Current.DisplayActionSheet(
                "Tipo de conta", "Cancelar", null,
                "Corrente", "Poupança", "Carteira", "Investimento", "Crédito");

            if (typeString == "Cancelar" || typeString is null) return;

            var accountType = typeString switch
            {
                "Poupança" => AccountType.Savings,
                "Carteira" => AccountType.Wallet,
                "Investimento" => AccountType.Investment,
                "Crédito" => AccountType.Credit,
                _ => AccountType.Checking
            };

            var account = new AccountLocal
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                AccountType = accountType,
                Balance = 0,
                InitialBalance = 0,
                LastBalanceUpdate = DateTime.UtcNow,
                IsDirty = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _local.SaveAccountAsync(account);
            await LoadAccountsAsync();
            await _sync.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar conta");
            await Shell.Current.DisplayAlert("Erro", "Não foi possível criar a conta", "OK");
        }
    }

    // -----------------------------------------------------
    // DELETE ACCOUNT (soft delete)
    // -----------------------------------------------------
    [RelayCommand]
    private async Task DeleteAccountAsync(AccountLocal account)
    {
        if (account is null) return;

        var confirm = await Shell.Current.DisplayAlert(
            "Excluir conta",
            $"Tem certeza que deseja remover \"{account.Name}\"?",
            "Sim, remover", "Não");

        if (!confirm) return;

        try
        {
            account.IsDeleted = true;
            account.IsDirty = true;
            account.UpdatedAt = DateTime.UtcNow;

            await _local.SaveAccountAsync(account);
            await LoadAccountsAsync();
            await _sync.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir conta");
            await Shell.Current.DisplayAlert("Erro", "Falha ao excluir a conta", "OK");
        }
    }
}