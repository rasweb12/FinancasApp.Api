// Services/LocalDatabase/TransactionLocalRepository.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public class TransactionLocalRepository : BaseRepository<TransactionLocal>
{
    public TransactionLocalRepository(SQLiteAsyncConnection db) : base(db)
    {
        db.ExecuteAsync(@"
            CREATE INDEX IF NOT EXISTS idx_transaction_date ON TransactionLocal(Date);
            CREATE INDEX IF NOT EXISTS idx_transaction_account ON TransactionLocal(AccountId);
        ").Wait();
    }

    public Task<List<TransactionLocal>> GetByAccountAsync(Guid accountId) =>
        _db.Table<TransactionLocal>()
           .Where(x => x.AccountId == accountId && !x.IsDeleted)
           .ToListAsync();
}