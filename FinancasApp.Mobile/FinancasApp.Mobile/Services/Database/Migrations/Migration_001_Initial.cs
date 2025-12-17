using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.Database;

public static class Migration_001_Initial
{
    public static async Task ApplyAsync(SQLiteAsyncConnection db)
    {
        await db.CreateTableAsync<AccountLocal>();
        await db.CreateTableAsync<TransactionLocal>();
        await db.CreateTableAsync<CreditCardLocal>();
        await db.CreateTableAsync<InvoiceLocal>();
        await db.CreateTableAsync<CategoryLocal>();
    }
}
