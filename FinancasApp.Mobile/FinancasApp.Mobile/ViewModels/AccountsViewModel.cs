// ViewModels/AccountsViewModel.cs — VERSÃO FINAL E CORRETA
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
    private readonly ISyncService _sync; // ← CORRETO: usa a interface
    private readonly ILogger<AccountsViewModel> _logger;

    [ObservableProperty] private ObservableCollection<AccountLocal> accounts = new();
    [ObservableProperty] private decimal totalBalance;
    [ObservableProperty] private bool isBusy;

    public AccountsViewModel(
        ILocalStorageService local,
        ISyncService sync, // ← CORRIGIDO AQUI
        ILogger<AccountsViewModel> logger)
    {
        _local = local;
        _sync = sync;
        _logger = logger;
    }

    [RelayCommand]
    private async Task LoadAccountsAsync()
    {
        IsBusy = true;
        try
        {
            var list = await _local.GetAccountsAsync() ?? new List<AccountLocal>();
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

    [RelayCommand]
    private async Task AddAccountAsync()
    {
        try
        {
            var name = await Shell.Current.DisplayPromptAsync("Nova Conta", "Nome da conta:", placeholder: "Ex: Nubank");
            if (string.IsNullOrWhiteSpace(name)) return;

            var typeString = await Shell.Current.DisplayActionSheet("Tipo de conta", "Cancelar", null,
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
                IsDirty = true,
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