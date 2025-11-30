// Services/Sync/SyncService.cs
using FinancasApp.Mobile.Mappers;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Services.Storage;
using Microsoft.Extensions.Logging;

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
        _logger.LogInformation("Sincronização iniciada — FinancasApp");
        try
        {
            await UploadPendingAsync();
            await DownloadUpdatesAsync();
            _logger.LogInformation("Sincronização concluída com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro crítico na sincronização");
            throw;
        }
    }

    // ==============================================================
    // UPLOAD: Envia tudo que está IsDirty ou IsDeleted
    // ==============================================================
    private async Task UploadPendingAsync()
    {
        // Contas
        var dirtyAccounts = (await _local.GetAccountsAsync())
            .Where(a => a.IsDirty || a.IsDeleted)
            .ToList();

        if (dirtyAccounts.Any())
        {
            await _api.SyncAccountsAsync(dirtyAccounts.Select(AccountMapper.ToDto).ToList());
            foreach (var a in dirtyAccounts) await MarkAsSyncedAsync(a);
        }

        // Transações
        var dirtyTransactions = (await _local.GetTransactionsAsync())
            .Where(t => t.IsDirty || t.IsDeleted)
            .ToList();

        if (dirtyTransactions.Any())
        {
            await _api.SyncTransactionsAsync(dirtyTransactions.Select(TransactionLocalMapper.ToDto).ToList());
            foreach (var t in dirtyTransactions) await MarkAsSyncedAsync(t);
        }

        // Cartões de Crédito
        var dirtyCards = (await _local.GetCreditCardsAsync())
            .Where(c => c.IsDirty || c.IsDeleted)
            .ToList();

        if (dirtyCards.Any())
        {
            await _api.SyncCardsAsync(dirtyCards.Select(CreditCardMapper.ToDto).ToList());
            foreach (var c in dirtyCards) await MarkAsSyncedAsync(c);
        }

        // Faturas
        var dirtyInvoices = await _local.GetPendingInvoicesAsync();
        if (dirtyInvoices.Any())
        {
            await _api.SyncInvoicesAsync(dirtyInvoices.Select(InvoiceMapper.ToDto).ToList());
            foreach (var i in dirtyInvoices) await MarkAsSyncedAsync(i);
        }
    }

    // ==============================================================
    // Marca como sincronizado (limpa flag e deleta se necessário)
    // ==============================================================
    private async Task MarkAsSyncedAsync(BaseEntity entity)
    {
        if (entity.IsDeleted)
        {
            // Usamos os métodos específicos porque BaseEntity é abstract
            switch (entity)
            {
                case AccountLocal a: await _local.DeleteAsync<AccountLocal>(a.Id); break;
                case TransactionLocal t: await _local.DeleteAsync<TransactionLocal>(t.Id); break;
                case CreditCardLocal c: await _local.DeleteAsync<CreditCardLocal>(c.Id); break;
                case InvoiceLocal i: await _local.DeleteAsync<InvoiceLocal>(i.Id); break;
            }
        }
        else
        {
            entity.IsDirty = false;
            entity.UpdatedAt = DateTime.UtcNow;

            // Usamos os métodos específicos (não SaveAsync<T> genérico)
            switch (entity)
            {
                case AccountLocal a: await _local.SaveAccountAsync(a); break;
                case TransactionLocal t: await _local.SaveTransactionAsync(t); break;
                case CreditCardLocal c: await _local.SaveCreditCardAsync(c); break;
                case InvoiceLocal i: await _local.SaveInvoiceAsync(i); break;
            }
        }
    }

    // ==============================================================
    // DOWNLOAD: Atualiza o banco local com dados do servidor
    // ==============================================================
    private async Task DownloadUpdatesAsync()
    {
        await Task.WhenAll(
            SyncAccountsAsync(),
            SyncTransactionsAsync(),
            SyncCreditCardsAsync(),
            SyncInvoicesAsync()
        );
    }

    private async Task SyncAccountsAsync()
    {
        var server = await _api.GetAccountsForSyncAsync();
        var local = await _local.GetAccountsAsync();

        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);

            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null) await _local.DeleteAsync<AccountLocal>(dto.Id);
                continue;
            }

            var updated = AccountMapper.ToLocal(dto);

            if (existing == null)
                await _local.SaveAccountAsync(updated);
            else if (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty)
                await _local.SaveAccountAsync(updated);
        }
    }

    private async Task SyncTransactionsAsync()
    {
        var server = await _api.GetTransactionsForSyncAsync();
        var local = await _local.GetTransactionsAsync();

        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);

            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null) await _local.DeleteAsync<TransactionLocal>(dto.Id);
                continue;
            }

            var updated = TransactionLocalMapper.ToLocal(dto);

            if (existing == null)
                await _local.SaveTransactionAsync(updated);
            else if (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty)
                await _local.SaveTransactionAsync(updated);
        }
    }

    private async Task SyncCreditCardsAsync()
    {
        var server = await _api.GetCardsForSyncAsync();
        var local = await _local.GetCreditCardsAsync();

        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);

            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null) await _local.DeleteAsync<CreditCardLocal>(dto.Id);
                continue;
            }

            var updated = CreditCardMapper.ToLocal(dto);

            if (existing == null)
                await _local.SaveCreditCardAsync(updated);
            else if (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty)
                await _local.SaveCreditCardAsync(updated);
        }
    }

    private async Task SyncInvoicesAsync()
    {
        var server = await _api.GetInvoicesForSyncAsync();
        var local = await _local.GetInvoicesAsync();

        foreach (var dto in server)
        {
            var existing = local.FirstOrDefault(x => x.Id == dto.Id);

            if (dto.IsDeleted && (existing == null || !existing.IsDirty))
            {
                if (existing != null) await _local.DeleteAsync<InvoiceLocal>(dto.Id);
                continue;
            }

            var updated = InvoiceMapper.ToLocal(dto);

            if (existing == null)
                await _local.SaveInvoiceAsync(updated);
            else if (dto.UpdatedAt > existing.UpdatedAt && !existing.IsDirty)
                await _local.SaveInvoiceAsync(updated);
        }
    }
}