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
            var list = await _local.GetAllAccountsAsync();

            Accounts.Clear();
            foreach (var a in list)
                Accounts.Add(a);

            TotalBalance = list.Sum(a => a.Balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar contas");
            await Application.Current!.MainPage!.DisplayAlert(
                "Erro",
                "Falha ao carregar contas",
                "OK");
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
            var name = await Application.Current!.MainPage!.DisplayPromptAsync(
                "Nova Conta", "Nome da conta:");

            if (string.IsNullOrWhiteSpace(name)) return;

            var typeString = await Shell.Current.DisplayActionSheet(
                "Tipo de conta", "Cancelar", null,
                "Corrente", "Poupança", "Carteira", "Investimento", "Crédito");

            if (typeString == "Cancelar") return;

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
                //IsNew = true,
                IsDirty = true
            };

            await _local.SaveAccountAsync(account);
            await LoadAccountsAsync();
            await _sync.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar conta");
        }
    }

    // -----------------------------------------------------
    // DELETE ACCOUNT (marcando como IsDeleted)
    // -----------------------------------------------------
    [RelayCommand]
    private async Task DeleteAccountAsync(AccountLocal account)
    {
        var confirm = await Application.Current!.MainPage!.DisplayAlert(
            "Excluir",
            $"Remover {account.Name}?",
            "Sim", "Não");

        if (!confirm) return;

        try
        {
            account.IsDeleted = true;
            account.IsDirty = true;

            await _local.SaveAccountAsync(account);
            await LoadAccountsAsync();
            await _sync.SyncAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir conta");
        }
    }
}
