// Services/Storage/ILocalStorageService.cs
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Services.Storage;

/// <summary>
/// Interface principal de armazenamento local (SQLite + Offline-First)
/// Contém métodos genéricos + específicos (necessários para o SyncService funcionar)
/// </summary>
public interface ILocalStorageService
{
    // ==============================================================
    // CRUD Genérico (usado em alguns lugares, mas NÃO no SyncService)
    // ==============================================================
    Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity, new();
    Task<List<T>> GetAllAsync<T>() where T : BaseEntity, new();
    Task<int> SaveAsync<T>(T entity) where T : BaseEntity, new();
    Task<int> DeleteAsync<T>(Guid id) where T : BaseEntity, new();

    // ==============================================================
    // MÉTODOS ESPECÍFICOS — OBRIGATÓRIOS PARA O SYNCSERVICE FUNCIONAR
    // ==============================================================
    Task<List<TransactionLocal>> GetTransactionsAsync();
    Task<int> SaveTransactionAsync(TransactionLocal transaction);
    Task DeleteTransactionAsync(Guid id); // ← NOVO: FALTAVA!

    Task<List<AccountLocal>> GetAccountsAsync();
    Task<int> SaveAccountAsync(AccountLocal account);
    Task DeleteAccountAsync(Guid id); // ← NOVO: FALTAVA!

    Task<List<CreditCardLocal>> GetCreditCardsAsync();
    Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id);
    Task<int> SaveCreditCardAsync(CreditCardLocal card);
    Task DeleteCreditCardAsync(Guid id); // ← NOVO: FALTAVA!

    Task<List<InvoiceLocal>> GetInvoicesAsync();
    Task<List<InvoiceLocal>> GetPendingInvoicesAsync();
    Task<InvoiceLocal?> GetCurrentInvoiceAsync();
    Task<int> SaveInvoiceAsync(InvoiceLocal invoice);
    Task DeleteInvoiceAsync(Guid id); // ← NOVO: FALTAVA!
}