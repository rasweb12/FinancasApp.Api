namespace FinancasApp.Api.DTOs;

public class SyncResponseDto
{
    public List<AccountDto> Accounts { get; set; } = new();
    public List<CreditCardDto> CreditCards { get; set; } = new();
    public List<InvoiceDto> Invoices { get; set; } = new();
    public List<TransactionDto> Transactions { get; set; } = new();
}

