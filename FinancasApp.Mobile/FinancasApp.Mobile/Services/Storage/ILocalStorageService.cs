// Services/Storage/ILocalStorageService.cs
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Services.Storage;

public interface ILocalStorageService
{
    // Accounts
    Task<AccountLocal?> GetAccountByIdAsync(Guid id);
    Task<List<AccountLocal>> GetAllAccountsAsync();
    Task<int> SaveAccountAsync(AccountLocal account);

    // Transactions
    Task<TransactionLocal?> GetTransactionByIdAsync(Guid id);
    Task<List<TransactionLocal>> GetAllTransactionsAsync();
    Task<int> SaveTransactionAsync(TransactionLocal transaction);

    // Credit Cards
    Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id);
    Task<List<CreditCardLocal>> GetAllCreditCardsAsync();
    Task<int> SaveCreditCardAsync(CreditCardLocal card);

    // Invoices
    Task<InvoiceLocal?> GetInvoiceByIdAsync(Guid id);
    Task<List<InvoiceLocal>> GetAllInvoicesAsync();
    Task<List<InvoiceLocal>> GetPendingInvoicesAsync();
    Task<int> SaveInvoiceAsync(InvoiceLocal invoice);
}