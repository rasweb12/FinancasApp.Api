using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.Storage;

public interface ILocalStorageService
{
    // Retorna a conexão (para repositórios especializados)
    SQLiteAsyncConnection GetConnection();

    // ======================================================================
    // ACCOUNTS
    // ======================================================================
    Task SaveAccountAsync(AccountLocal account);
    Task<List<AccountLocal>> GetAccountsAsync();
    Task<AccountLocal?> GetAccountByIdAsync(Guid id);
    Task DeleteAccountAsync(Guid id);

    // ======================================================================
    // TRANSACTIONS
    // ======================================================================
    Task SaveTransactionAsync(TransactionLocal trx);
    Task<List<TransactionLocal>> GetTransactionsAsync();
    Task<List<TransactionLocal>> GetTransactionsByAccountAsync(Guid accountId);
    Task<List<TransactionLocal>> GetPendingTransactionsAsync();
    Task<TransactionLocal?> GetTransactionByIdAsync(Guid id);
    Task DeleteTransactionAsync(Guid id);

    // ======================================================================
    // CREDIT CARDS
    // ======================================================================
    Task SaveCreditCardAsync(CreditCardLocal card);
    Task<List<CreditCardLocal>> GetCreditCardsAsync();
    Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id);
    Task DeleteCreditCardAsync(Guid id);

    // ======================================================================
    // INVOICES
    // ======================================================================
    Task SaveInvoiceAsync(InvoiceLocal invoice);
    Task<List<InvoiceLocal>> GetInvoicesAsync();
    Task<List<InvoiceLocal>> GetInvoicesByCardAsync(Guid cardId);
    Task<InvoiceLocal?> GetCurrentInvoiceAsync(Guid cardId, int month, int year);
    Task<InvoiceLocal?> GetInvoiceByIdAsync(Guid id);
    Task DeleteInvoiceAsync(Guid id);

    // ======================================================================
    // SYNC
    // ======================================================================
    Task<List<object>> GetPendingSyncItemsAsync();
}
