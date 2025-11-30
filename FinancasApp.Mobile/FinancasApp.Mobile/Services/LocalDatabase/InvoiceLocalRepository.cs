// Services/LocalDatabase/InvoiceLocalRepository.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public class InvoiceLocalRepository : BaseRepository<InvoiceLocal>
{
    public InvoiceLocalRepository(SQLiteAsyncConnection db) : base(db) { }

    public async Task<List<InvoiceLocal>> GetDirtyAsync()
        => await _db.Table<InvoiceLocal>().Where(x => x.IsDirty && !x.IsDeleted).ToListAsync();
}