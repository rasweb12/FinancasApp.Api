using SQLite;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Data
{
    public class InvoiceLocalRepository : BaseRepository<InvoiceLocal>
    {
        public InvoiceLocalRepository(SQLiteAsyncConnection db) : base(db)
        {
            _db.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_invoice_creditcard ON InvoiceLocal(CreditCardId);").Wait();
        }

        public Task<InvoiceLocal?> GetInvoiceAsync(Guid cardId, int month, int year)
            => _db.Table<InvoiceLocal>()
                  .Where(x => x.CreditCardId == cardId
                           && x.Month == month
                           && x.Year == year
                           && !x.IsDeleted)
                  .FirstOrDefaultAsync();
    }
}
