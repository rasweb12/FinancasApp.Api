// Services/Storage/ILocalStorageService.cs
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Services.Storage;

public interface ILocalStorageService
{
    // === CRUD Genérico ===
    Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity, new();
    Task<List<T>> GetAllAsync<T>() where T : BaseEntity, new();
    Task<int> SaveAsync<T>(T entity) where T : BaseEntity, new();
    Task<int> DeleteAsync<T>(Guid id) where T : BaseEntity, new();

    // === Métodos específicos (OBRIGATÓRIOS para consistência) ===
    Task<List<TransactionLocal>> GetTransactionsAsync();
    Task<int> SaveTransactionAsync(TransactionLocal transaction);

    Task<List<AccountLocal>> GetAccountsAsync();
    Task<int> SaveAccountAsync(AccountLocal account);

    Task<List<CreditCardLocal>> GetCreditCardsAsync();
    Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id);
    Task<int> SaveCreditCardAsync(CreditCardLocal card); // ← AQUI ESTAVA FALTANDO!!!

    Task<List<InvoiceLocal>> GetInvoicesAsync();
    Task<List<InvoiceLocal>> GetPendingInvoicesAsync();
    Task<InvoiceLocal?> GetCurrentInvoiceAsync();
    Task<int> SaveInvoiceAsync(InvoiceLocal invoice);
}