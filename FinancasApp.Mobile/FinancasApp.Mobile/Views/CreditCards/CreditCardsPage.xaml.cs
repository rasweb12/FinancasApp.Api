// Views/CreditCards/CreditCardsPage.xaml.cs
using FinancasApp.Mobile.ViewModels;

namespace FinancasApp.Mobile.Views.CreditCards;

public partial class CreditCardsPage : ContentPage
{
    private readonly CreditCardsViewModel _vm;

    public CreditCardsPage(CreditCardsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCardsCommand.ExecuteAsync(null);
    }
}