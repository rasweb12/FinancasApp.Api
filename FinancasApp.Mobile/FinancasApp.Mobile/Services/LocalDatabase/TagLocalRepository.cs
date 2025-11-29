using SQLite;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Data
{
    public class TagLocalRepository : BaseRepository<TagLocal>
    {
        public TagLocalRepository(SQLiteAsyncConnection db) : base(db)
        {
            _db.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_tag_name ON TagLocal(Name);").Wait();
        }
    }
}
