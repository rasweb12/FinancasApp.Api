// Services/LocalDatabase/Repository.cs
using FinancasApp.Mobile.Models.Local;
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public interface IRepository<T> where T : BaseEntity, new()
{
    Task<T?> GetByIdAsync(Guid id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetDirtyAsync();     // ← usa IsDirty
    Task<List<T>> GetDeletedAsync();   // ← usa IsDeleted
    Task<int> SaveAsync(T entity);
    Task<int> DeleteAsync(T entity);
    Task<int> DeletePermanentlyAsync(Guid id);
}

public class Repository<T> : IRepository<T> where T : BaseEntity, new()
{
    protected readonly SQLiteAsyncConnection _db;

    public Repository(SQLiteAsyncConnection db) => _db = db;

    public Task<T?> GetByIdAsync(Guid id) => _db.GetAsync<T>(id);

    public Task<List<T>> GetAllAsync() => _db.Table<T>().Where(x => !x.IsDeleted).ToListAsync();

    public Task<List<T>> GetDirtyAsync() =>
        _db.Table<T>().Where(x => x.IsDirty && !x.IsDeleted).ToListAsync();

    public Task<List<T>> GetDeletedAsync() =>
        _db.Table<T>().Where(x => x.IsDeleted).ToListAsync();

    public async Task<int> SaveAsync(T entity)
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

    public async Task<int> DeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        entity.IsDirty = true;
        entity.UpdatedAt = DateTime.UtcNow;
        return await _db.UpdateAsync(entity);
    }

    public async Task<int> DeletePermanentlyAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
            return await _db.DeleteAsync(entity);
        return 0;
    }
}