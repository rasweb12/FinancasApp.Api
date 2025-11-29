using SQLite;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Data
{
    public class CreditCardLocalRepository : BaseRepository<CreditCardLocal>
    {
        public CreditCardLocalRepository(SQLiteAsyncConnection db) : base(db)
        {
            _db.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_creditcard_name ON CreditCardLocal(Name);").Wait();
        }
    }
}
