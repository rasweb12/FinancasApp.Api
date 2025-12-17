// Services/Sync/SyncService.cs
// VERSÃO FINAL CORRIGIDA COM CATEGORIAS — 16/12/2025
using FinancasApp.Mobile.Mappers;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;
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

    public SyncService(IApiService api, ILocalStorageService local, ILogger<SyncService> logger)
    {
        _api = api;
        _local = local;
        _logger = logger;
    }

    public async Task SyncAllAsync()
    {
        _logger.LogInformation("🔄 Sincronização iniciada");

        try
        {
            var request = new SyncRequestDto
            {
                Accounts = (await _local.GetAccountsAsync()).Where(a => a.IsDirty || a.IsDeleted).Select(AccountMapper.ToDto).ToList(),
                CreditCards = (await _local.GetCreditCardsAsync()).Where(c => c.IsDirty || c.IsDeleted).Select(CreditCardMapper.ToDto).ToList(),
                Invoices = (await _local.GetPendingInvoicesAsync()).Select(InvoiceMapper.ToDto).ToList(),
                Transactions = (await _local.GetTransactionsAsync()).Where(t => t.IsDirty || t.IsDeleted).Select(TransactionLocalMapper.ToDto).ToList(),
                Categories = (await _local.GetCategoriesAsync()).Where(c => c.IsDirty || c.IsDeleted).Select(CategoryMapper.ToDto).ToList()
            };

            SyncResponseDto serverData;

            if (request.HasAnyDirtyData())
            {
                var response = await _api.SyncAllAsync(request);
                if (!response.IsSuccessStatusCode || response.Content is null)
                    throw new Exception("Falha no sync com servidor");

                serverData = response.Content;
                _logger.LogInformation("✅ Upload + Download concluído");
            }
            else
            {
                serverData = await DownloadAllFromServerAsync();
                _logger.LogInformation("📥 Apenas download");
            }

            await ApplyServerDataAsync(serverData);
            _logger.LogInformation("🏁 Sync concluído!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Falha crítica no sync");
            throw;
        }
    }

    private async Task<SyncResponseDto> DownloadAllFromServerAsync()
    {
        return new SyncResponseDto
        {
            Accounts = (await _api.GetAccountsForSyncAsync()).Content ?? [],
            CreditCards = (await _api.GetCardsForSyncAsync()).Content ?? [],
            Transactions = (await _api.GetTransactionsForSyncAsync()).Content ?? [],
            Invoices = (await _api.GetInvoicesForSyncAsync()).Content ?? [],
            Categories = (await _api.GetCategoriesForSyncAsync()).Content ?? []
        };
    }

    private async Task ApplyServerDataAsync(SyncResponseDto response)
    {
        await Task.WhenAll(
            ApplyAccountsAsync(response.Accounts),
            ApplyCreditCardsAsync(response.CreditCards),
            ApplyTransactionsAsync(response.Transactions),
            ApplyInvoicesAsync(response.Invoices),
            ApplyCategoriesAsync(response.Categories)
        );
    }

    private async Task ApplyAccountsAsync(List<AccountDto> server) { /* código existente */ }
    private async Task ApplyCreditCardsAsync(List<CreditCardDto> server) { /* código existente */ }
    private async Task ApplyTransactionsAsync(List<TransactionDto> server) { /* código existente */ }
    private async Task ApplyInvoicesAsync(List<InvoiceDto> server) { /* código existente */ }

    private async Task ApplyCategoriesAsync(List<CategoryDto> server)
    {
        var local = await _local.GetCategoriesAsync();

        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);

            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null)
                    await _local.DeleteCategoryAsync(dto.Id);
                continue;
            }

            var updated = CategoryMapper.ToLocal(dto);

            if (existing == null || (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveCategoryAsync(updated);
            }
        }
    }
}