using SQLite;

namespace FinancasApp.Mobile.Services.Database;

public static class DatabaseInitializer
{
    private const int CurrentVersion = 2; // aumente sempre que criar nova migração

    public static async Task InitializeAsync(SQLiteAsyncConnection db)
    {
        // Cria tabela de metadados se não existir
        await db.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS __Meta (
                Key TEXT PRIMARY KEY,
                Value TEXT
            );
        ");

        // Lê versão atual
        var versionString = await db.ExecuteScalarAsync<string>(
            "SELECT Value FROM __Meta WHERE Key = 'DbVersion'"
        );

        int oldVersion = int.TryParse(versionString, out int v) ? v : 0;

        // Executar migrações incrementais
        for (int next = oldVersion + 1; next <= CurrentVersion; next++)
        {
            await ApplyMigrationAsync(db, next);
        }

        // Atualiza versão
        await db.ExecuteAsync(
            "INSERT OR REPLACE INTO __Meta (Key, Value) VALUES ('DbVersion', ?)",
            CurrentVersion.ToString()
        );
    }

    private static async Task ApplyMigrationAsync(SQLiteAsyncConnection db, int version)
    {
        switch (version)
        {
            case 1:
                await Migration_001_Initial.ApplyAsync(db);
                break;

            case 2:
                await Migration_002_AddCardStatement.ApplyAsync(db);
                break;

            default:
                break;
        }
    }
}
