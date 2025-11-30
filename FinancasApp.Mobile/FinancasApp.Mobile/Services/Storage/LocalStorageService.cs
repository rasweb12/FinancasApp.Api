using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.Storage
{
    public class LocalStorageService : ILocalStorageService
    {
        private readonly SQLiteAsyncConnection _db;

        public LocalStorageService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await _db.CreateTableAsync<AccountLocal>();
            await _db.CreateTableAsync<TransactionLocal>();
            await _db.CreateTableAsync<CreditCardLocal>();
            await _db.CreateTableAsync<InvoiceLocal>();
        }

        // -------------------- ACCOUNTS --------------------
        public Task<AccountLocal?> GetAccountByIdAsync(Guid id)
            => _db.Table<AccountLocal>().Where(x => x.Id == id).FirstOrDefaultAsync();

        public Task<List<AccountLocal>> GetAllAccountsAsync()
            => _db.Table<AccountLocal>().ToListAsync();

        public Task<int> SaveAccountAsync(AccountLocal account)
            => _db.InsertOrReplaceAsync(account);

        // -------------------- TRANSACTIONS --------------------
        public Task<TransactionLocal?> GetTransactionByIdAsync(Guid id)
            => _db.Table<TransactionLocal>().Where(x => x.Id == id).FirstOrDefaultAsync();

        public Task<List<TransactionLocal>> GetAllTransactionsAsync()
            => _db.Table<TransactionLocal>().ToListAsync();

        public Task<int> SaveTransactionAsync(TransactionLocal transaction)
            => _db.InsertOrReplaceAsync(transaction);

        // -------------------- CREDIT CARDS --------------------
        public Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id)
            => _db.Table<CreditCardLocal>().Where(x => x.Id == id).FirstOrDefaultAsync();

        public Task<List<CreditCardLocal>> GetAllCreditCardsAsync()
            => _db.Table<CreditCardLocal>().ToListAsync();

        public Task<int> SaveCreditCardAsync(CreditCardLocal card)
            => _db.InsertOrReplaceAsync(card);

        // -------------------- INVOICES --------------------
        public Task<InvoiceLocal?> GetInvoiceByIdAsync(Guid id)
            => _db.Table<InvoiceLocal>().Where(x => x.Id == id).FirstOrDefaultAsync();

        public Task<List<InvoiceLocal>> GetAllInvoicesAsync()
            => _db.Table<InvoiceLocal>().ToListAsync();

        public Task<List<InvoiceLocal>> GetPendingInvoicesAsync()
            => _db.Table<InvoiceLocal>().Where(x => !x.IsPaid).ToListAsync();

        public Task<int> SaveInvoiceAsync(InvoiceLocal invoice)
            => _db.InsertOrReplaceAsync(invoice);
    }
}
