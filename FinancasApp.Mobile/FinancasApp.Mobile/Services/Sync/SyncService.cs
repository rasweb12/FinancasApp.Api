// Services/Sync/SyncService.cs
using FinancasApp.Mobile.Mappers;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.Storage;
using Microsoft.Extensions.Logging;
using Refit;

namespace FinancasApp.Mobile.Services.Sync;

public interface ISyncService
{
    Task SyncAllAsync();
}

public class SyncService : ISyncService
{
    private readonly IApiService _api;
    private readonly ILocalStorageService _local;
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        IApiService api,
        ILocalStorageService local,
        ILogger<SyncService> logger)
    {
        _api = api;
        _local = local;
        _logger = logger;
    }

    public async Task SyncAllAsync()
    {
        _logger.LogInformation("🔄 Sincronização iniciada — FinancasApp");

        try
        {
            var request = new SyncRequestDto
            {
                Accounts = (await _local.GetAccountsAsync())
                    .Where(a => a.IsDirty || a.IsDeleted)
                    .Select(AccountMapper.ToDto)
                    .ToList(),

                CreditCards = (await _local.GetCreditCardsAsync())
                    .Where(c => c.IsDirty || c.IsDeleted)
                    .Select(CreditCardMapper.ToDto)
                    .ToList(),

                Invoices = (await _local.GetPendingInvoicesAsync())
                    .Select(InvoiceMapper.ToDto)
                    .ToList(),

                Transactions = (await _local.GetTransactionsAsync())
                    .Where(t => t.IsDirty || t.IsDeleted)
                    .Select(TransactionLocalMapper.ToDto)
                    .ToList()
            };

            SyncResponseDto serverData;

            // 🔼 Upload + 🔽 Download
            if (request.Accounts.Any() ||
                request.CreditCards.Any() ||
                request.Invoices.Any() ||
                request.Transactions.Any())
            {
                var apiResponse = await _api.SyncAllAsync(request);

                if (!apiResponse.IsSuccessStatusCode || apiResponse.Content is null)
                {
                    _logger.LogError(
                        "Erro no POST /sync | Status: {Status} | Content: {Content}",
                        apiResponse.StatusCode,
                        apiResponse.Error?.Content);

                    throw new Exception("Falha ao sincronizar dados com o servidor.");
                }

                serverData = apiResponse.Content;

                _logger.LogInformation("✅ Upload + Download concluído via POST /sync");
            }
            else
            {
                _logger.LogInformation("📥 Download puro (nenhum dado local para upload)");

                serverData = new SyncResponseDto
                {
                    Accounts = (await _api.GetAccountsForSyncAsync()).Content ?? [],
                    CreditCards = (await _api.GetCardsForSyncAsync()).Content ?? [],
                    Transactions = (await _api.GetTransactionsForSyncAsync()).Content ?? [],
                    Invoices = (await _api.GetInvoicesForSyncAsync()).Content ?? []
                };
            }

            await ApplyServerDataAsync(serverData);

            _logger.LogInformation("🏁 Sincronização concluída com sucesso!");
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(apiEx,
                "Erro HTTP na sincronização | StatusCode: {StatusCode} | Content: {Content}",
                apiEx.StatusCode,
                apiEx.Content);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro crítico na sincronização");
            throw;
        }
    }

    // =========================
    // APPLY SERVER DATA
    // =========================

    private async Task ApplyServerDataAsync(SyncResponseDto response)
    {
        await Task.WhenAll(
            ApplyAccountsAsync(response.Accounts),
            ApplyCreditCardsAsync(response.CreditCards),
            ApplyTransactionsAsync(response.Transactions),
            ApplyInvoicesAsync(response.Invoices)
        );
    }

    private async Task ApplyAccountsAsync(List<AccountDto> server)
    {
        var local = await _local.GetAccountsAsync();

        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);

            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null)
                    await _local.DeleteAccountAsync(dto.Id);

                continue;
            }

            var updated = AccountMapper.ToLocal(dto);

            if (existing == null ||
                (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveAccountAsync(updated);
            }
        }
    }

    private async Task ApplyCreditCardsAsync(List<CreditCardDto> server)
    {
        var local = await _local.GetCreditCardsAsync();

        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);

            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null)
                    await _local.DeleteCreditCardAsync(dto.Id);

                continue;
            }

            var updated = CreditCardMapper.ToLocal(dto);

            if (existing == null ||
                (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveCreditCardAsync(updated);
            }
        }
    }

    private async Task ApplyTransactionsAsync(List<TransactionDto> server)
    {
        var local = await _local.GetTransactionsAsync();

        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);

            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null)
                    await _local.DeleteTransactionAsync(dto.Id);

                continue;
            }

            var updated = TransactionLocalMapper.ToLocal(dto);

            if (existing == null ||
                (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveTransactionAsync(updated);
            }
        }
    }

    private async Task ApplyInvoicesAsync(List<InvoiceDto> server)
    {
        var local = await _local.GetInvoicesAsync();

        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);

            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null)
                    await _local.DeleteInvoiceAsync(dto.Id);

                continue;
            }

            var updated = InvoiceMapper.ToLocal(dto);

            if (existing == null ||
                (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveInvoiceAsync(updated);
            }
        }
    }
}
