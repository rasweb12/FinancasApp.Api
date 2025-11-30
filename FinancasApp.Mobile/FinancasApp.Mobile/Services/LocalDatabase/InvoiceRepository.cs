// Services/LocalDatabase/InvoiceRepository.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public class InvoiceRepository : BaseRepository<InvoiceLocal>
{
    public InvoiceRepository(SQLiteAsyncConnection db) : base(db) { }

    public Task<List<InvoiceLocal>> GetPendingAsync() =>
        _db.Table<InvoiceLocal>()
           .Where(i => i.SyncStatus == SyncStatus.Pending)
           .ToListAsync();
}