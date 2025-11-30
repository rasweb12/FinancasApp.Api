// Services/Sync/TransactionSyncService.cs
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.LocalDatabase;
using FinancasApp.Mobile.Extensions;

namespace FinancasApp.Mobile.Services.Sync;

public interface ITransactionSyncService
{
    Task SyncAsync();
}

public class TransactionSyncService : ITransactionSyncService
{
    private readonly TransactionLocalRepository _repo;
    private readonly ITransactionApiService _api; // ← Interface correta (vamos criar)

    public TransactionSyncService(
        TransactionLocalRepository repo,
        ITransactionApiService api)
    {
        _repo = repo;
        _api = api;
    }

    public async Task SyncAsync()
    {
        await PushAsync();
        await PullAsync();
    }

    private async Task PushAsync()
    {
        var dirty = await _repo.GetDirtyAsync();
        foreach (var local in dirty)
        {
            try
            {
                if (local.IsDeleted)
                {
                    await _api.DeleteAsync(local.Id);
                }
                else
                {
                    var dto = local.ToDto(); // ← Precisa existir (vamos garantir)
                    await _api.UpsertAsync(dto);
                }

                local.IsDirty = false;
                await _repo.SaveAsync(local);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TransactionSync] Falha ao sincronizar {local.Id}: {ex.Message}");
            }
        }
    }

    private async Task PullAsync()
    {
        var serverTransactions = await _api.GetAllAsync();
        foreach (var dto in serverTransactions)
        {
            var local = await _repo.GetByIdAsync(dto.Id);

            if (local == null)
            {
                var novo = dto.ToLocal();
                await _repo.SaveAsync(novo);
                continue;
            }

            if (dto.UpdatedAt > local.UpdatedAt)
            {
                local.UpdateFromDto(dto);
                local.IsDirty = false;
                await _repo.SaveAsync(local);
            }
        }
    }
}