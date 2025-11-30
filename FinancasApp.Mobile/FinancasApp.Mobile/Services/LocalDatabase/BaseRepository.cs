// Services/LocalDatabase/BaseRepository.cs
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public abstract class BaseRepository<T> where T : class, new()
{
    protected readonly SQLiteAsyncConnection _db;

    public BaseRepository(SQLiteAsyncConnection db)
    {
        _db = db;
    }

    public Task<List<T>> GetAllAsync() => _db.Table<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _db.FindAsync<T>(id);
    }

    public Task<int> InsertAsync(T entity) => _db.InsertAsync(entity);
    public Task<int> UpdateAsync(T entity) => _db.UpdateAsync(entity);
    public Task<int> DeleteAsync(T entity) => _db.DeleteAsync(entity);
}