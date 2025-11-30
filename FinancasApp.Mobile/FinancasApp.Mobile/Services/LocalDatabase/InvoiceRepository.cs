// Services/LocalDatabase/InvoiceRepository.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public class InvoiceRepository : BaseRepository<InvoiceLocal>
{
    public InvoiceRepository(SQLiteAsyncConnection db) : base(db) { }

    // Agora usa o padrão IsDirty do seu BaseEntity
    public Task<List<InvoiceLocal>> GetPendingAsync() =>
        _db.Table<InvoiceLocal>()
           .Where(i => i.IsDirty && !i.IsDeleted)
           .ToListAsync();
}