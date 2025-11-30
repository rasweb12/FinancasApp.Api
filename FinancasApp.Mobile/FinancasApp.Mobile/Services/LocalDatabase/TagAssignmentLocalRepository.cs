// Services/LocalDatabase/TagAssignmentLocalRepository.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public class TagAssignmentLocalRepository : BaseRepository<TagAssignmentLocal>
{
    public TagAssignmentLocalRepository(SQLiteAsyncConnection db) : base(db)
    {
        db.ExecuteAsync(@"
            CREATE INDEX IF NOT EXISTS idx_tagassign_tx ON TagAssignmentLocal(TransactionId);
            CREATE INDEX IF NOT EXISTS idx_tagassign_tag ON TagAssignmentLocal(TagId);
        ").Wait();
    }

    public Task<List<TagAssignmentLocal>> GetTagsForTransactionAsync(Guid txId) =>
        _db.Table<TagAssignmentLocal>()
           .Where(x => x.TransactionId == txId && !x.IsDeleted)
           .ToListAsync();
}