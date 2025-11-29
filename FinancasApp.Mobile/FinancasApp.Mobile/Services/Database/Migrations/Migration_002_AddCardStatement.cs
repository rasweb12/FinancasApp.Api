using SQLite;

namespace FinancasApp.Mobile.Services.Database;

public static class Migration_002_AddCardStatement
{
    public static async Task ApplyAsync(SQLiteAsyncConnection db)
    {
        // Exemplo de coluna nova
        await db.ExecuteAsync(@"
            ALTER TABLE InvoiceLocal ADD COLUMN StatementMonth INTEGER DEFAULT 0
        ");
    }
}
