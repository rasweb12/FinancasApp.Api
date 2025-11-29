// InvoiceRepository.cs (CardStatement)
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Data;
using FinancasApp.Mobile.Services;
using FinancasApp.Mobile.Services.Storage;


namespace FinancasApp.Mobile.Data;


public interface ICardStatementRepository : IBaseRepository<InvoiceLocal>
{
    Task<List<InvoiceLocal>> GetByCardIdAsync(Guid cardId);
}


public interface IBaseRepository<T>
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<int> InsertAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(T entity);
}



public class CardStatementRepository : BaseRepository<InvoiceLocal>, ICardStatementRepository
{
    public CardStatementRepository(ILocalStorageService storage)
        : base(storage) { }  // <<< Agora a BaseRepository recebe o ILocalStorageService

    public Task<List<InvoiceLocal>> GetByCardIdAsync(Guid cardId) =>
        _db.Table<InvoiceLocal>()
           .Where(i => i.CreditCardId == cardId)
           .ToListAsync();
}
