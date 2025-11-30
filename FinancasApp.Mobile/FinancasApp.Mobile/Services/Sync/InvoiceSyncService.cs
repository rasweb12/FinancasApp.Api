// Services/Sync/InvoiceSyncService.cs
using FinancasApp.Mobile.Mappers;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.LocalDatabase;

namespace FinancasApp.Mobile.Services.Sync;

public interface IInvoiceSyncService
{
    Task SyncAsync();
}

public class InvoiceSyncService : IInvoiceSyncService
{
    private readonly IRepository<InvoiceLocal> _repo;
    private readonly InvoiceApiService _api;

    public InvoiceSyncService(IRepository<InvoiceLocal> repo, InvoiceApiService api)
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
                    var dto = local.ToDto();
                    await _api.UpsertAsync(dto);
                }

                local.IsDirty = false;
                await _repo.SaveAsync(local);
            }
            catch (Exception ex)
            {
                // Log do erro (não interrompe o sync)
                Console.WriteLine($"Erro sync invoice {local.Id}: {ex.Message}");
            }
        }
    }

    private async Task PullAsync()
    {
        var server = await _api.GetAllAsync();
        foreach (var dto in server)
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