// Services/Storage/LocalStorageService.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.Storage;

public class LocalStorageService : ILocalStorageService
{
    private readonly SQLiteAsyncConnection _db;

    public LocalStorageService(SQLiteAsyncConnection db)
    {
        _db = db;
    }

    // CRUD Genérico
    public async Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity, new()
        => await _db.FindAsync<T>(id);

    public async Task<List<T>> GetAllAsync<T>() where T : BaseEntity, new()
        => await _db.Table<T>().ToListAsync();

    public async Task<int> SaveAsync<T>(T entity) where T : BaseEntity, new()
    {
        entity.IsDirty = true;
        entity.UpdatedAt = DateTime.UtcNow;

        var existing = await _db.FindAsync<T>(entity.Id);
        return existing is null
            ? await _db.InsertAsync(entity)
            : await _db.UpdateAsync(entity);
    }

    public async Task<int> DeleteAsync<T>(Guid id) where T : BaseEntity, new()
    {
        var entity = await GetByIdAsync<T>(id);
        return entity is not null ? await _db.DeleteAsync(entity) : 0;
    }

    // Métodos específicos
    public Task<List<TransactionLocal>> GetTransactionsAsync() => GetAllAsync<TransactionLocal>();
    public Task<int> SaveTransactionAsync(TransactionLocal t) => SaveAsync(t);

    public Task<List<AccountLocal>> GetAccountsAsync() => GetAllAsync<AccountLocal>();
    public Task<int> SaveAccountAsync(AccountLocal a) => SaveAsync(a);

    public Task<List<CreditCardLocal>> GetCreditCardsAsync() => GetAllAsync<CreditCardLocal>();
    public Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id) => GetByIdAsync<CreditCardLocal>(id);

    public Task<List<InvoiceLocal>> GetInvoicesAsync() => GetAllAsync<InvoiceLocal>();
    public async Task<List<InvoiceLocal>> GetPendingInvoicesAsync() =>
        await _db.Table<InvoiceLocal>().Where(i => i.IsDirty || i.IsDeleted).ToListAsync();

    public async Task<InvoiceLocal?> GetCurrentInvoiceAsync() =>
        (await GetInvoicesAsync()).OrderByDescending(i => i.CreatedAt).FirstOrDefault();

    public Task<int> SaveInvoiceAsync(InvoiceLocal i) => SaveAsync(i);
}