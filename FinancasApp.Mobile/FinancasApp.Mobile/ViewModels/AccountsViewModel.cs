//ViewModels/AccountsViewModel.cs
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

    public AccountsViewModel(ILocalStorageService local, SyncService sync, ILogger<AccountsViewModel> logger)
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
            var list = await _local.GetAccountsAsync();
            Accounts.Clear();
            foreach (var a in list)
                Accounts.Add(a);

            TotalBalance = list.Sum(a => a.Balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar contas");
            await Application.Current!.MainPage!.DisplayAlert("Erro", "Falha ao carregar contas", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddAccountAsync()
    {
        var name = await Application.Current!.MainPage!.DisplayPromptAsync("Nova Conta", "Nome da conta:");
        if (string.IsNullOrWhiteSpace(name)) return;

        // CORRETO: método existe
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
            Name = name,
            AccountType = accountType,
            Balance = 0
        };

        await _local.SaveAccountAsync(account);
        await LoadAccountsCommand.ExecuteAsync(null);
        await _sync.SyncAllAsync();
    }


    [RelayCommand]
    private async Task DeleteAccountAsync(AccountLocal account)
    {
        var confirm = await Application.Current!.MainPage!.DisplayAlert("Excluir", $"Remover {account.Name}?", "Sim", "Não");
        if (!confirm) return;

        await _local.DeleteAccountAsync(account.Id);
        await LoadAccountsCommand.ExecuteAsync(null);
        await _sync.SyncAllAsync();
    }
}