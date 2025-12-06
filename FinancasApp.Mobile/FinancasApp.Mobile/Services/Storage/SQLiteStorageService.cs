// Services/Storage/SQLiteStorageService.cs — VERSÃO FINAL, LIMPA, MODERNA E IMORTAL
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.LocalDatabase;

namespace FinancasApp.Mobile.Services.Storage;

public class SQLiteStorageService : ILocalStorageService
{
    private readonly IRepository<AccountLocal> _accountRepo;
    private readonly IRepository<TransactionLocal> _transactionRepo;
    private readonly IRepository<CreditCardLocal> _creditCardRepo;
    private readonly IRepository<InvoiceLocal> _invoiceRepo;
    private readonly IRepository<TagLocal> _tagRepo;
    private readonly IRepository<TagAssignmentLocal> _tagAssignmentRepo;

    public SQLiteStorageService(
        IRepository<AccountLocal> accountRepo,
        IRepository<TransactionLocal> transactionRepo,
        IRepository<CreditCardLocal> creditCardRepo,
        IRepository<InvoiceLocal> invoiceRepo,
        IRepository<TagLocal> tagRepo,
        IRepository<TagAssignmentLocal> tagAssignmentRepo)
    {
        _accountRepo = accountRepo;
        _transactionRepo = transactionRepo;
        _creditCardRepo = creditCardRepo;
        _invoiceRepo = invoiceRepo;
        _tagRepo = tagRepo;
        _tagAssignmentRepo = tagAssignmentRepo;
    }

    // === CRUD Genérico 100% Funcional (agora com IRepository<T>) ===
    public async Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity, new()
    {
        return typeof(T) switch
        {
            var t when t == typeof(AccountLocal) => await _accountRepo.GetByIdAsync(id) as T,
            var t when t == typeof(TransactionLocal) => await _transactionRepo.GetByIdAsync(id) as T,
            var t when t == typeof(CreditCardLocal) => await _creditCardRepo.GetByIdAsync(id) as T,
            var t when t == typeof(InvoiceLocal) => await _invoiceRepo.GetByIdAsync(id) as T,
            var t when t == typeof(TagLocal) => await _tagRepo.GetByIdAsync(id) as T,
            var t when t == typeof(TagAssignmentLocal) => await _tagAssignmentRepo.GetByIdAsync(id) as T,
            _ => null
        };
    }

    public async Task<List<T>> GetAllAsync<T>() where T : BaseEntity, new()
    {
        return typeof(T) switch
        {
            var t when t == typeof(AccountLocal) => (await _accountRepo.GetAllAsync()).Cast<T>().ToList(),
            var t when t == typeof(TransactionLocal) => (await _transactionRepo.GetAllAsync()).Cast<T>().ToList(),
            var t when t == typeof(CreditCardLocal) => (await _creditCardRepo.GetAllAsync()).Cast<T>().ToList(),
            var t when t == typeof(InvoiceLocal) => (await _invoiceRepo.GetAllAsync()).Cast<T>().ToList(),
            var t when t == typeof(TagLocal) => (await _tagRepo.GetAllAsync()).Cast<T>().ToList(),
            var t when t == typeof(TagAssignmentLocal) => (await _tagAssignmentRepo.GetAllAsync()).Cast<T>().ToList(),
            _ => new List<T>()
        };
    }

    public async Task<int> SaveAsync<T>(T entity) where T : BaseEntity, new()
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsDirty = true;

        return entity switch
        {
            AccountLocal a => await _accountRepo.SaveAsync(a),
            TransactionLocal t => await _transactionRepo.SaveAsync(t),
            CreditCardLocal c => await _creditCardRepo.SaveAsync(c),
            InvoiceLocal i => await _invoiceRepo.SaveAsync(i),
            TagLocal tag => await _tagRepo.SaveAsync(tag),
            TagAssignmentLocal ta => await _tagAssignmentRepo.SaveAsync(ta),
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
            TagLocal tag => await _tagRepo.DeleteAsync(tag),
            TagAssignmentLocal ta => await _tagAssignmentRepo.DeleteAsync(ta),
            _ => 0
        };
    }

    // === Métodos específicos (mantidos por compatibilidade) ===
    public Task<List<TransactionLocal>> GetTransactionsAsync() => _transactionRepo.GetAllAsync();
    public Task<int> SaveTransactionAsync(TransactionLocal t) => SaveAsync(t);
    public Task<List<AccountLocal>> GetAccountsAsync() => _accountRepo.GetAllAsync();
    public Task<int> SaveAccountAsync(AccountLocal a) => SaveAsync(a);
    public Task<List<CreditCardLocal>> GetCreditCardsAsync() => _creditCardRepo.GetAllAsync();
    public Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id) => _creditCardRepo.GetByIdAsync(id);
    public Task<List<InvoiceLocal>> GetInvoicesAsync() => _invoiceRepo.GetAllAsync();
    public Task<List<InvoiceLocal>> GetPendingInvoicesAsync() => _invoiceRepo.GetDirtyAsync();
    public Task<InvoiceLocal?> GetCurrentInvoiceAsync() =>
        _invoiceRepo.GetAllAsync().ContinueWith(t => t.Result.OrderByDescending(i => i.CreatedAt).FirstOrDefault());
    public Task<int> SaveInvoiceAsync(InvoiceLocal i) => SaveAsync(i);
    public Task<int> SaveCreditCardAsync(CreditCardLocal c) => SaveAsync(c);
}