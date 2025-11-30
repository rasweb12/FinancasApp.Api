// Services/Storage/SQLiteStorageService.cs
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.LocalDatabase;

namespace FinancasApp.Mobile.Services.Storage;

public class SQLiteStorageService : ILocalStorageService
{
    private readonly IBaseRepository<AccountLocal> _accountRepo;
    private readonly IBaseRepository<TransactionLocal> _transactionRepo;
    private readonly IBaseRepository<CreditCardLocal> _creditCardRepo;
    private readonly IBaseRepository<InvoiceLocal> _invoiceRepo;

    public SQLiteStorageService(
        IBaseRepository<AccountLocal> accountRepo,
        IBaseRepository<TransactionLocal> transactionRepo,
        IBaseRepository<CreditCardLocal> creditCardRepo,
        IBaseRepository<InvoiceLocal> invoiceRepo)
    {
        _accountRepo = accountRepo;
        _transactionRepo = transactionRepo;
        _creditCardRepo = creditCardRepo;
        _invoiceRepo = invoiceRepo;
    }

    // === CRUD Genérico (agora 100% funcional e sem CS0704) ===
    public async Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity, new()
    {
        return typeof(T) switch
        {
            var t when t == typeof(AccountLocal) => await _accountRepo.GetByIdAsync(id) as T,
            var t when t == typeof(TransactionLocal) => await _transactionRepo.GetByIdAsync(id) as T,
            var t when t == typeof(CreditCardLocal) => await _creditCardRepo.GetByIdAsync(id) as T,
            var t when t == typeof(InvoiceLocal) => await _invoiceRepo.GetByIdAsync(id) as T,
            _ => null
        };
    }

    public async Task<List<T>> GetAllAsync<T>() where T : BaseEntity, new()
    {
        var result = typeof(T) switch
        {
            var t when t == typeof(AccountLocal) => (await _accountRepo.GetAllAsync()).Cast<T>().ToList(),
            var t when t == typeof(TransactionLocal) => (await _transactionRepo.GetAllAsync()).Cast<T>().ToList(),
            var t when t == typeof(CreditCardLocal) => (await _creditCardRepo.GetAllAsync()).Cast<T>().ToList(),
            var t when t == typeof(InvoiceLocal) => (await _invoiceRepo.GetAllAsync()).Cast<T>().ToList(),
            _ => new List<T>()
        };
        return result;
    }

    public async Task<int> SaveAsync<T>(T entity) where T : BaseEntity, new()
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.IsDirty = true;
        entity.UpdatedAt = DateTime.UtcNow;

        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        return entity switch
        {
            AccountLocal a => entity.Id == Guid.Empty
                ? await _accountRepo.InsertAsync(a)
                : await _accountRepo.UpdateAsync(a),

            TransactionLocal t => entity.Id == Guid.Empty
                ? await _transactionRepo.InsertAsync(t)
                : await _transactionRepo.UpdateAsync(t),

            CreditCardLocal c => entity.Id == Guid.Empty
                ? await _creditCardRepo.InsertAsync(c)
                : await _creditCardRepo.UpdateAsync(c),

            InvoiceLocal i => entity.Id == Guid.Empty
                ? await _invoiceRepo.InsertAsync(i)
                : await _invoiceRepo.UpdateAsync(i),

            _ => throw new NotSupportedException($"Tipo {typeof(T)} não suportado")
        };
    }

    public async Task<int> DeleteAsync<T>(Guid id) where T : BaseEntity, new()
    {
        var entity = await GetByIdAsync<T>(id);
        if (entity == null) return 0;

        return entity switch
        {
            AccountLocal a => await _accountRepo.DeleteAsync(a),
            TransactionLocal t => await _transactionRepo.DeleteAsync(t),
            CreditCardLocal c => await _creditCardRepo.DeleteAsync(c),
            InvoiceLocal i => await _invoiceRepo.DeleteAsync(i),
            _ => 0
        };
    }

    // === Métodos específicos (mantidos por compatibilidade com ViewModels) ===
    public Task<List<TransactionLocal>> GetTransactionsAsync() => _transactionRepo.GetAllAsync();
    public Task<int> SaveTransactionAsync(TransactionLocal t) => SaveAsync(t);

    public Task<List<AccountLocal>> GetAccountsAsync() => _accountRepo.GetAllAsync();
    public Task<int> SaveAccountAsync(AccountLocal a) => SaveAsync(a);

    public Task<List<CreditCardLocal>> GetCreditCardsAsync() => _creditCardRepo.GetAllAsync();
    public Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id) => _creditCardRepo.GetByIdAsync(id);

    public Task<List<InvoiceLocal>> GetInvoicesAsync() => _invoiceRepo.GetAllAsync();
    public async Task<List<InvoiceLocal>> GetPendingInvoicesAsync()
    {
        if (_invoiceRepo is InvoiceRepository repo)
            return await repo.GetPendingAsync();

        return (await _invoiceRepo.GetAllAsync())
            .Where(i => i.IsDirty || i.IsDeleted)
            .ToList();
    }

    public async Task<InvoiceLocal?> GetCurrentInvoiceAsync()
        => (await GetInvoicesAsync())
            .OrderByDescending(i => i.CreatedAt)
            .FirstOrDefault();

    public Task<int> SaveInvoiceAsync(InvoiceLocal i) => SaveAsync(i);
}