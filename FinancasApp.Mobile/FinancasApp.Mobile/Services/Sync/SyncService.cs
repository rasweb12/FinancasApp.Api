using FinancasApp.Mobile.Mappers;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.Auth;
using FinancasApp.Mobile.Services.Storage;
using Microsoft.Extensions.Logging;
using Refit;
using System.Net;

namespace FinancasApp.Mobile.Services.Sync;

public interface ISyncService
{
    Task SyncAllAsync();
}

public class SyncService : ISyncService
{
    private readonly IApiService _api;
    private readonly ILocalStorageService _local;
    private readonly IAuthService _auth;
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        IApiService api,
        ILocalStorageService local,
        IAuthService auth,
        ILogger<SyncService> logger)
    {
        _api = api;
        _local = local;
        _auth = auth;
        _logger = logger;
    }

    public async Task SyncAllAsync()
    {
        _logger.LogInformation("🔄 Sync iniciado");

        try
        {
            var request = new SyncRequestDto
            {
                Accounts = (await _local.GetAccountsAsync())
                    .Where(x => x.IsDirty || x.IsDeleted)
                    .Select(AccountMapper.ToDto)
                    .ToList(),
                CreditCards = (await _local.GetCreditCardsAsync())
                    .Where(x => x.IsDirty || x.IsDeleted)
                    .Select(CreditCardMapper.ToDto)
                    .ToList(),
                Transactions = (await _local.GetTransactionsAsync())
                    .Where(x => x.IsDirty || x.IsDeleted)
                    .Select(TransactionLocalMapper.ToDto)
                    .ToList(),
                Invoices = (await _local.GetPendingInvoicesAsync())
                    .Select(InvoiceMapper.ToDto)
                    .ToList(),
                Categories = (await _local.GetCategoriesAsync())
                    .Where(x => x.IsDirty || x.IsDeleted)
                    .Select(CategoryMapper.ToDto)
                    .ToList()
            };

            SyncResponseDto serverData;
            _logger.LogInformation("📤 Dados dirty para upload: Categories={Count}", request.Categories.Count);

            if (request.HasAnyDirtyData())
            {
                _logger.LogInformation("⬆️ Enviando {Count} categorias dirty", request.Categories.Count);

                var response = await _api.SyncAllAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await HandleUnauthorizedAsync();
                    return;
                }

                if (!response.IsSuccessStatusCode || response.Content is null)
                {
                    _logger.LogError("Falha no sync | Status: {Status}", response.StatusCode);
                    throw new Exception("Falha no servidor");
                }

                _logger.LogInformation("⬆️⬇️ Sync sucesso | Recebido do servidor");
            }
            else
            {
                _logger.LogInformation("⬇️ Nenhum dado dirty — apenas download");
                serverData = await DownloadAllAsync();
                _logger.LogInformation("⬇️ Apenas download");
            }

            await ApplyServerDataAsync(serverData);
            _logger.LogInformation("✅ Sync concluído com sucesso");
        }
        catch (ApiException apiEx) when (apiEx.StatusCode == HttpStatusCode.Unauthorized) // ◄ CORRIGIDO
        {
            await HandleUnauthorizedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro no sync");
            throw;
        }
    }

    private async Task HandleUnauthorizedAsync()
    {
        _logger.LogWarning("Token inválido ou expirado (401) — forçando logout");
        await _auth.LogoutAsync();
        await Shell.Current.DisplayAlert("Sessão Expirada", "Sua sessão expirou. Faça login novamente.", "OK");
        await Shell.Current.GoToAsync("//login");
    }

    private async Task<SyncResponseDto> DownloadAllAsync()
    {
        var accounts = await SafeCall(() => _api.GetAccountsForSyncAsync());
        var creditCards = await SafeCall(() => _api.GetCardsForSyncAsync());
        var transactions = await SafeCall(() => _api.GetTransactionsForSyncAsync());
        var invoices = await SafeCall(() => _api.GetInvoicesForSyncAsync());
        var categories = await SafeCall(() => _api.GetCategoriesForSyncAsync());

        return new SyncResponseDto
        {
            Accounts = accounts ?? new(),
            CreditCards = creditCards ?? new(),
            Transactions = transactions ?? new(),
            Invoices = invoices ?? new(),
            Categories = categories ?? new()
        };
    }

    private async Task<List<T>?> SafeCall<T>(Func<Task<ApiResponse<List<T>>>> call)
    {
        try
        {
            var response = await call();
            return response.Content;
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized) // ◄ CORRIGIDO
        {
            await HandleUnauthorizedAsync();
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao baixar dados");
            return null;
        }
    }

    private async Task ApplyServerDataAsync(SyncResponseDto response)
    {
        await Task.WhenAll(
            ApplyAccountsAsync(response.Accounts ?? new()),
            ApplyCreditCardsAsync(response.CreditCards ?? new()),
            ApplyTransactionsAsync(response.Transactions ?? new()),
            ApplyInvoicesAsync(response.Invoices ?? new()),
            ApplyCategoriesAsync(response.Categories ?? new())
        );
    }

    // Métodos Apply (iguais ao anterior, com IsDirty = false no final)
    private async Task ApplyAccountsAsync(List<AccountDto> server)
    {
        var local = await _local.GetAccountsAsync();
        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);
            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null) await _local.DeleteAccountAsync(dto.Id);
                continue;
            }

            var updated = AccountMapper.ToLocal(dto);
            if (existing == null || (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveAccountAsync(updated);
            }

            if (existing != null) existing.IsDirty = false;
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
                if (existing != null) await _local.DeleteCreditCardAsync(dto.Id);
                continue;
            }

            var updated = CreditCardMapper.ToLocal(dto);
            if (existing == null || (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveCreditCardAsync(updated);
            }

            if (existing != null) existing.IsDirty = false;
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
                if (existing != null) await _local.DeleteTransactionAsync(dto.Id);
                continue;
            }

            var updated = TransactionLocalMapper.ToLocal(dto);
            if (existing == null || (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveTransactionAsync(updated);
            }

            if (existing != null) existing.IsDirty = false;
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
                if (existing != null) await _local.DeleteInvoiceAsync(dto.Id);
                continue;
            }

            var updated = InvoiceMapper.ToLocal(dto);
            if (existing == null || (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveInvoiceAsync(updated);
            }

            if (existing != null) existing.IsDirty = false;
        }
    }

    private async Task ApplyCategoriesAsync(List<CategoryDto> server)
    {
        var local = await _local.GetCategoriesAsync();
        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);
            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null) await _local.DeleteCategoryAsync(dto.Id);
                continue;
            }

            var updated = CategoryMapper.ToLocal(dto);
            if (existing == null || (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty))
            {
                await _local.SaveCategoryAsync(updated);
            }

            if (existing != null) existing.IsDirty = false;
        }
    }
}