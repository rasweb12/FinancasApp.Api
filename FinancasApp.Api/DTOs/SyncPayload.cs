namespace FinancasApp.Api.DTOs;

public class SyncPayload
{
    public List<AccountDto> Accounts { get; set; } = new();
    public List<TransactionDto> Transactions { get; set; } = new();
    public List<CreditCardDto> CreditCards { get; set; } = new();
    public List<InvoiceDto> Invoices { get; set; } = new();
}

