namespace FinancasApp.Api.DTOs;

public class SyncRequestDto
{
    public List<AccountDto> Accounts { get; set; } = new();
    public List<CreditCardDto> CreditCards { get; set; } = new();
    public List<InvoiceDto> Invoices { get; set; } = new();
    public List<TransactionDto> Transactions { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
}


