using FinancasApp.Mobile.Views.CreditCards;
using FinancasApp.Mobile.Views.Dashboard;
using FinancasApp.Mobile.Views.Reports;
using FinancasApp.Mobile.Views.Transactions;

namespace FinancasApp.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // 🔗 Rotas internas (não aparecem no menu)
        Routing.RegisterRoute("newtransaction", typeof(NewTransactionPage));
        Routing.RegisterRoute("cardinvoice", typeof(InvoiceDetailPage));
        Routing.RegisterRoute(nameof(NewTransactionPage), typeof(NewTransactionPage));

    }

    // 🔗 Exemplo: navegar com parâmetro
    public static async Task NavigateToInvoiceAsync(Guid cardId)
    {
        await Shell.Current.GoToAsync("cardinvoice", new Dictionary<string, object>
        {
            ["cardId"] = cardId
        });
    }
}
