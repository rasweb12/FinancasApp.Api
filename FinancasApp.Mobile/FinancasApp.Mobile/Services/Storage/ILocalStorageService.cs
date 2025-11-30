// Services/Storage/ILocalStorageService.cs
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Services.Storage;

public interface ILocalStorageService
{
    // === CRUD Genérico (funciona com qualquer entidade que herde de BaseEntity ===
    Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity, new();
    Task<List<T>> GetAllAsync<T>() where T : BaseEntity, new();
    Task<int> SaveAsync<T>(T entity) where T : BaseEntity, new();
    Task<int> DeleteAsync<T>(Guid id) where T : BaseEntity, new();

    // === Métodos específicos que você realmente usa nos ViewModels ===
    Task<List<TransactionLocal>> GetTransactionsAsync();
    Task<int> SaveTransactionAsync(TransactionLocal transaction);

    Task<List<AccountLocal>> GetAccountsAsync();
    Task<int> SaveAccountAsync(AccountLocal account);

    Task<List<CreditCardLocal>> GetCreditCardsAsync();
    Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id);

    Task<List<InvoiceLocal>> GetInvoicesAsync();
    Task<List<InvoiceLocal>> GetPendingInvoicesAsync(); // ← mantido exatamente como você pediu
    Task<InvoiceLocal?> GetCurrentInvoiceAsync();
    Task<int> SaveInvoiceAsync(InvoiceLocal invoice);
}