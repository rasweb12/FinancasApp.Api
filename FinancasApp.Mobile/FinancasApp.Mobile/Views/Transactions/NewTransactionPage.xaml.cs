// Views/Transactions/NewTransactionPage.xaml.cs
using FinancasApp.Mobile.ViewModels;

namespace FinancasApp.Mobile.Views.Transactions;

public partial class NewTransactionPage : ContentPage
{
    private readonly NewTransactionViewModel _vm;

    public NewTransactionPage(NewTransactionViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }
}