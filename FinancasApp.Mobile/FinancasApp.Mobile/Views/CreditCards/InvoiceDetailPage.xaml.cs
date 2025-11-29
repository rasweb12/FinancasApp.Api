// Views/CreditCards/InvoiceDetailPage.xaml.cs
using FinancasApp.Mobile.ViewModels;
using Microsoft.Maui.Controls; // ESSENCIAL

namespace FinancasApp.Mobile.Views.CreditCards;

public partial class InvoiceDetailPage : ContentPage, IQueryAttributable
{
    private readonly InvoiceDetailViewModel _vm;

    public InvoiceDetailPage(InvoiceDetailViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // CORRETO: QueryAttributes agora existe
        if (QueryAttributes.TryGetValue("cardId", out var idObj) &&
            Guid.TryParse(idObj?.ToString(), out var cardId))
        {
            await _vm.InitializeAsync(cardId);
        }
    }

    // IMPLEMENTAÇÃO OBRIGATÓRIA
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        QueryAttributes = query;
    }

    // PROPRIEDADE OBRIGATÓRIA
    public IDictionary<string, object> QueryAttributes { get; private set; } = new Dictionary<string, object>();
}