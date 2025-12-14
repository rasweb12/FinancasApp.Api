using FinancasApp.Mobile.ViewModels;
using FinancasApp.Mobile.Views.CreditCards;
using FinancasApp.Mobile.Views.Transactions;

namespace FinancasApp.Mobile;

public partial class AppShell : Shell
{
    private readonly AppShellViewModel _viewModel;

    public AppShell(AppShellViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // 🔗 Rotas internas
        Routing.RegisterRoute("newtransaction", typeof(NewTransactionPage));
        Routing.RegisterRoute("cardinvoice", typeof(InvoiceDetailPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 🔐 Decide tela inicial
        if (_viewModel.IsAuthenticated)
            await GoToAsync("//home");
        else
            await GoToAsync("//login");
    }

    // ✅ MÉTODO QUE ESTAVA FALTANDO
    public static Task NavigateToInvoiceAsync(Guid cardId)
    {
        return Shell.Current.GoToAsync("cardinvoice", new Dictionary<string, object>
        {
            ["cardId"] = cardId
        });
    }
}
