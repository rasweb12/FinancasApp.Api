// Services/LocalDatabase/CreditCardLocalRepository.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public class CreditCardLocalRepository : BaseRepository<CreditCardLocal>
{
    public CreditCardLocalRepository(SQLiteAsyncConnection db) : base(db)
    {
        // Índice para busca rápida por nome
        db.ExecuteAsync(@"
            CREATE INDEX IF NOT EXISTS idx_creditcard_name 
            ON CreditCardLocal(Name);").Wait();
    }
}