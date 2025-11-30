// Services/LocalDatabase/BaseRepository.cs
using SQLite;

namespace FinancasApp.Mobile.Services.LocalDatabase;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class, new()
{
    protected readonly SQLiteAsyncConnection _db;

    public BaseRepository(SQLiteAsyncConnection db)
    {
        _db = db;
    }

    public async Task<List<T>> GetAllAsync() => await _db.Table<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _db.Table<T>().FirstOrDefaultAsync(x =>
            EF.Property<Guid>(x, "Id") == id);
    }

    public async Task<int> InsertAsync(T entity) => await _db.InsertAsync(entity);

    public async Task<int> UpdateAsync(T entity) => await _db.UpdateAsync(entity);

    public async Task<int> DeleteAsync(T entity) => await _db.DeleteAsync(entity);

    public async Task<int> DeleteByIdAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        return entity != null ? await DeleteAsync(entity) : 0;
    }
}