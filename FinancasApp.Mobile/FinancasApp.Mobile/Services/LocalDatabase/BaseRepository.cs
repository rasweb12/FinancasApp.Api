// Services/LocalDatabase/BaseRepository.cs
using SQLite;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public abstract class BaseRepository<T> where T : BaseEntity, new()
{
    protected readonly SQLiteAsyncConnection _db;

    protected BaseRepository(SQLiteAsyncConnection db)
    {
        _db = db;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
        => await _db.GetAsync<T>(id);

    public virtual async Task<List<T>> GetAllAsync()
        => await _db.Table<T>().Where(x => !x.IsDeleted).ToListAsync();

    public virtual async Task<List<T>> GetDirtyAsync()
        => await _db.Table<T>().Where(x => x.IsDirty && !x.IsDeleted).ToListAsync();

    public virtual async Task<int> SaveAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsDirty = true;

        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            return await _db.InsertAsync(entity);
        }
        return await _db.UpdateAsync(entity);
    }

    public virtual async Task<int> SoftDeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        entity.IsDirty = true;
        entity.UpdatedAt = DateTime.UtcNow;
        return await _db.UpdateAsync(entity);
    }
}