// SecureStorageService.cs
using Microsoft.Maui.Storage;


namespace FinancasApp.Mobile.Services;


public interface ISecureStorageService
{
    Task SaveAsync(string key, string value);
    Task<string?> GetAsync(string key);
    void Remove(string key);
    void Clear();
}


public class SecureStorageService : ISecureStorageService
{
    public Task SaveAsync(string key, string value) => SecureStorage.Default.SetAsync(key, value);
    public Task<string?> GetAsync(string key) => SecureStorage.Default.GetAsync(key);
    public void Remove(string key) => SecureStorage.Default.Remove(key);
    public void Clear() => SecureStorage.Default.RemoveAll();
}