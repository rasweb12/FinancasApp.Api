using SQLite;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Data
{
    public class BaseRepository<T> where T : BaseEntity, new()
    {
        protected readonly SQLiteAsyncConnection _db;

        public BaseRepository(SQLiteAsyncConnection db)
        {
            _db = db;
            _db.CreateTableAsync<T>().Wait();
        }

        public Task<List<T>> GetAllAsync()
            => _db.Table<T>().Where(x => !x.IsDeleted).ToListAsync();

        public Task<T?> GetByIdAsync(Guid id)
            => _db.Table<T>().Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();

        public async Task<int> InsertAsync(T entity)
        {
            entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            return await _db.InsertAsync(entity).ConfigureAwait(false);
        }

        public async Task<int> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            return await _db.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task<int> UpsertAsync(T entity)
        {
            var existing = await GetByIdAsync(entity.Id).ConfigureAwait(false);
            if (existing == null)
                return await InsertAsync(entity);
            else
                return await UpdateAsync(entity);
        }

        public async Task<int> SoftDeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id).ConfigureAwait(false);
            if (entity == null) return 0;

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;

            return await _db.UpdateAsync(entity);
        }

        public Task<List<T>> GetDirtyAsync()
            => _db.Table<T>().Where(x => x.IsDirty || x.IsDeleted).ToListAsync();

        public async Task<int> MarkAsSyncedAsync(Guid id)
        {
            var entity = await GetByIdAsync(id).ConfigureAwait(false);
            if (entity == null) return 0;

            entity.IsDirty = false;
            return await _db.UpdateAsync(entity);
        }

        public Task<int> InsertManyAsync(IEnumerable<T> entities)
        {
            foreach (var e in entities)
            {
                e.CreatedAt = DateTime.UtcNow;
                e.UpdatedAt = DateTime.UtcNow;
            }
            return _db.InsertAllAsync(entities);
        }

        public Task<int> UpdateManyAsync(IEnumerable<T> entities)
        {
            foreach (var e in entities)
                e.UpdatedAt = DateTime.UtcNow;

            return _db.UpdateAllAsync(entities);
        }
    }
}
