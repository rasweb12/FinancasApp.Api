// Services/LocalDatabase/TagLocalRepository.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public class TagLocalRepository : Repository<TagLocal>
{
    public TagLocalRepository(SQLiteAsyncConnection db) : base(db)
    {
        db.ExecuteAsync(@"
            CREATE INDEX IF NOT EXISTS idx_tag_name 
            ON TagLocal(Name);").Wait();
    }
}