using FinancasApp.Mobile.ViewModels;

namespace FinancasApp.Mobile.Views.Accounts;

public partial class AccountsPage : ContentPage
{
    private readonly AccountsViewModel _vm;
    public AccountsPage(AccountsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAccountsCommand.ExecuteAsync(null);
    }
}