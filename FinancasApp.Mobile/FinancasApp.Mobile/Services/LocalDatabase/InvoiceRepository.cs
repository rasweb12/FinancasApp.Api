// Services/LocalDatabase/InvoiceRepository.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public class InvoiceRepository : BaseRepository<InvoiceLocal>, IBaseRepository<InvoiceLocal>
{
    public InvoiceRepository(SQLiteAsyncConnection db) : base(db)
    {
    }

    // Métodos específicos de Invoice (se precisar)
    public async Task<List<InvoiceLocal>> GetPendingAsync()
    {
        return await _db.Table<InvoiceLocal>()
            .Where(i => i.SyncStatus == SyncStatus.Pending)
            .ToListAsync();
    }
}