using SQLite;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Data
{
    public class TagAssignmentLocalRepository : BaseRepository<TagAssignmentLocal>
    {
        public TagAssignmentLocalRepository(SQLiteAsyncConnection db) : base(db)
        {
            _db.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_tagassign_tx ON TagAssignmentLocal(TransactionId);").Wait();
            _db.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_tagassign_tag ON TagAssignmentLocal(TagId);").Wait();
        }

        public Task<List<TagAssignmentLocal>> GetTagsForTransactionAsync(Guid txId)
            => _db.Table<TagAssignmentLocal>()
                  .Where(x => x.TransactionId == txId && !x.IsDeleted)
                  .ToListAsync();
    }
}
