using SQLite;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Data;
using FinancasApp.Mobile.Services.Storage;

namespace FinancasApp.Mobile.Services.Storage
{

    public class SQLiteStorageService : ILocalStorageService
    {
        private readonly SQLiteAsyncConnection _db;
        private bool _initialized = false;

        public SQLiteStorageService()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "financasapp.db3");

            _db = new SQLiteAsyncConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite |
                SQLiteOpenFlags.Create |
                SQLiteOpenFlags.SharedCache
            );
        }

        // ======================================================================
        // 1. Inicialização + Migrations
        // ======================================================================
        private async Task InitAsync()
        {
            if (_initialized) return;

            await _db.CreateTableAsync<AccountLocal>();
            await _db.CreateTableAsync<TransactionLocal>();
            await _db.CreateTableAsync<CreditCardLocal>();
            await _db.CreateTableAsync<InvoiceLocal>();

            _initialized = true;
        }

        // ======================================================================
        // 2. Métodos base genéricos
        // ======================================================================
        public async Task InsertAsync<T>(T entity) where T : class, new()
        {
            await InitAsync();
            await _db.InsertAsync(entity);
        }

        public async Task UpdateAsync<T>(T entity) where T : class, new()
        {
            await InitAsync();
            await _db.UpdateAsync(entity);
        }

        public async Task DeleteAsync<T>(T entity) where T : class, new()
        {
            await InitAsync();
            await _db.DeleteAsync(entity);
        }

        public async Task DeletePermanentAsync<T>(T entity) where T : class, new()
        {
            await InitAsync();
            await _db.DeleteAsync(entity);
        }

        public async Task<T?> GetByIdAsync<T>(Guid id) where T : class, new()
        {
            await InitAsync();
            return await _db.FindAsync<T>(id);
        }

        public async Task<List<T>> GetAllAsync<T>() where T : class, new()
        {
            await InitAsync();
            return await _db.Table<T>().ToListAsync();
        }

        // ======================================================================
        // 3. Accounts
        // ======================================================================
        public async Task SaveAccountAsync(AccountLocal account)
        {
            await InitAsync();

            if (await _db.FindAsync<AccountLocal>(account.Id) == null)
                await InsertAsync(account);
            else
                await UpdateAsync(account);
        }

        public Task<List<AccountLocal>> GetAccountsAsync() =>
            GetAllAsync<AccountLocal>();

        public async Task DeleteAccountAsync(Guid id)
        {
            await InitAsync();
            var item = await _db.FindAsync<AccountLocal>(id);
            if (item != null) await _db.DeleteAsync(item);
        }

        public async Task<List<AccountLocal>> GetPendingAccountsAsync()
        {
            await InitAsync();
            return await _db.Table<AccountLocal>()
                .Where(x => x.IsDirty || x.IsNew || x.IsDeleted)
                .ToListAsync();
        }

        // ======================================================================
        // 4. Transactions
        // ======================================================================
        public async Task SaveTransactionAsync(TransactionLocal trx)
        {
            await InitAsync();

            if (await _db.FindAsync<TransactionLocal>(trx.Id) == null)
                await InsertAsync(trx);
            else
                await UpdateAsync(trx);
        }

        public Task<List<TransactionLocal>> GetTransactionsAsync() =>
            GetAllAsync<TransactionLocal>();

        public async Task<List<TransactionLocal>> GetTransactionsByAccountAsync(Guid accountId)
        {
            await InitAsync();

            return await _db.Table<TransactionLocal>()
                .Where(t => t.Tags.Contains(accountId.ToString()))
                .ToListAsync();
        }

        public async Task DeleteTransactionAsync(Guid id)
        {
            await InitAsync();
            var item = await _db.FindAsync<TransactionLocal>(id);
            if (item != null) await _db.DeleteAsync(item);
        }

        public async Task<List<TransactionLocal>> GetPendingTransactionsAsync()
        {
            await InitAsync();
            return await _db.Table<TransactionLocal>()
                .Where(t => t.IsDirty || t.IsNew || t.IsDeleted)
                .ToListAsync();
        }

        // ======================================================================
        // 5. Credit Cards
        // ======================================================================
        public async Task SaveCreditCardAsync(CreditCardLocal card)
        {
            await InitAsync();

            if (await _db.FindAsync<CreditCardLocal>(card.Id) == null)
                await InsertAsync(card);
            else
                await UpdateAsync(card);
        }

        public Task<List<CreditCardLocal>> GetCreditCardsAsync() =>
            GetAllAsync<CreditCardLocal>();

        public async Task<CreditCardLocal?> GetCreditCardByIdAsync(Guid id)
        {
            await InitAsync();
            return await _db.FindAsync<CreditCardLocal>(id);
        }

        public async Task DeleteCreditCardAsync(Guid id)
        {
            await InitAsync();
            var item = await _db.FindAsync<CreditCardLocal>(id);
            if (item != null) await _db.DeleteAsync(item);
        }

        public async Task<List<CreditCardLocal>> GetPendingCreditCardsAsync()
        {
            await InitAsync();
            return await _db.Table<CreditCardLocal>()
                .Where(x => x.IsDirty || x.IsNew || x.IsDeleted)
                .ToListAsync();
        }

        // ======================================================================
        // 6. Invoices
        // ======================================================================
        public async Task SaveInvoiceAsync(InvoiceLocal invoice)
        {
            await InitAsync();

            if (await _db.FindAsync<InvoiceLocal>(invoice.Id) == null)
                await InsertAsync(invoice);
            else
                await UpdateAsync(invoice);
        }

        public async Task<List<InvoiceLocal>> GetInvoicesAsync()
        {
            await InitAsync();
            return await _db.Table<InvoiceLocal>().ToListAsync();
        }

        public async Task<List<InvoiceLocal>> GetInvoicesByCardAsync(Guid cardId)
        {
            await InitAsync();
            return await _db.Table<InvoiceLocal>()
                .Where(i => i.CreditCardId == cardId)
                .ToListAsync();
        }

        public async Task<InvoiceLocal?> GetCurrentInvoiceAsync(Guid cardId, int month, int year)
        {
            await InitAsync();

            return await _db.Table<InvoiceLocal>()
                .Where(i => i.CreditCardId == cardId &&
                            i.Month == month &&
                            i.Year == year)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteInvoiceAsync(Guid id)
        {
            await InitAsync();
            var item = await _db.FindAsync<InvoiceLocal>(id);
            if (item != null) await _db.DeleteAsync(item);
        }

        public async Task<List<InvoiceLocal>> GetPendingInvoicesAsync()
        {
            await InitAsync();
            return await _db.Table<InvoiceLocal>()
                .Where(x => x.IsDirty || x.IsNew || x.IsDeleted)
                .ToListAsync();
        }

        // ======================================================================
        // 7. MarkAsSynced — usado pelo SyncService
        // ======================================================================
        public async Task MarkAsSyncedAsync<T>(Guid id) where T : class, new()
        {
            await InitAsync();

            var entity = await _db.FindAsync<T>(id);

            if (entity is ISyncFlags syncEntity)
            {
                syncEntity.IsNew = false;
                syncEntity.IsDirty = false;
                syncEntity.IsDeleted = false;

                await _db.UpdateAsync(syncEntity);
            }
        }

        // ======================================================================
        // 8. Agrupador usado para debug
        // ======================================================================
        public async Task<List<object>> GetPendingSyncItemsAsync()
        {
            await InitAsync();

            var accounts = await GetPendingAccountsAsync();
            var transactions = await GetPendingTransactionsAsync();
            var cards = await GetPendingCreditCardsAsync();
            var invoices = await GetPendingInvoicesAsync();

            return new List<object>()
                .Concat(accounts)
                .Concat(transactions)
                .Concat(cards)
                .Concat(invoices)
                .ToList();
        }

        // ======================================================================
        // 9. Connection (para Repositórios)
        // ======================================================================
        public SQLiteAsyncConnection GetConnection() => _db;

    }
}