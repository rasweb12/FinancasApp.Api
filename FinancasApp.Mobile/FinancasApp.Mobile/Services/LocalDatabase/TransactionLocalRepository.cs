using SQLite;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Data
{
    public class TransactionLocalRepository : BaseRepository<TransactionLocal>
    {
        public TransactionLocalRepository(SQLiteAsyncConnection db) : base(db)
        {
            _db.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transaction_date ON TransactionLocal(Date);").Wait();
            _db.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_transaction_account ON TransactionLocal(AccountId);").Wait();
        }

        public Task<List<TransactionLocal>> GetByAccountAsync(Guid accountId)
            => _db.Table<TransactionLocal>()
                  .Where(x => x.AccountId == accountId && !x.IsDeleted)
                  .ToListAsync();
    }
}
