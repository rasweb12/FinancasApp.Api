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
        _logger.LogInformation("🔄 SyncService: início da sincronização.");

        try
        {
            await UploadPendingItemsAsync();
            await DownloadUpdatesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante SyncAllAsync");
        }
    }

    // ==============================================================
    // UPLOAD
    // ==============================================================

    private async Task UploadPendingItemsAsync()
    {
        _logger.LogInformation("⬆ UploadPendingItemsAsync: buscando itens pendentes...");

        var pending = await _local.GetPendingSyncItemsAsync();

        if (pending.Count == 0)
        {
            _logger.LogInformation("Nenhum item pendente de upload.");
            return;
        }

        var accounts = pending.OfType<AccountLocal>().ToList();
        var tx = pending.OfType<TransactionLocal>().ToList();
        var cards = pending.OfType<CreditCardLocal>().ToList();
        var invoices = pending.OfType<InvoiceLocal>().ToList();

        // ACCOUNTS
        if (accounts.Any())
        {
            try
            {
                await _api.SyncAccountsAsync(accounts.Select(ToDto).ToList());
                foreach (var a in accounts) await MarkAsSyncedAsync(a);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao enviar contas.");
            }
        }

        // TRANSACTIONS
        if (tx.Any())
        {
            try
            {
                await _api.SyncTransactionsAsync(tx.Select(TransactionLocalMapper.ToDto).ToList());
                foreach (var t in tx) await MarkAsSyncedAsync(t);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao enviar transações.");
            }
        }

        // CARDS
        if (cards.Any())
        {
            try
            {
                await _api.SyncCardsAsync(cards.Select(ToDto).ToList());
                foreach (var c in cards) await MarkAsSyncedAsync(c);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao enviar cartões.");
            }
        }

        // INVOICES
        if (invoices.Any())
        {
            try
            {
                await _api.SyncInvoicesAsync(invoices.Select(ToDto).ToList());
                foreach (var i in invoices) await MarkAsSyncedAsync(i);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha ao enviar faturas.");
            }
        }
    }

    private async Task MarkAsSyncedAsync(BaseLocalEntity entity)
    {
        if (entity == null) return;

        entity.IsNew = false;
        entity.IsDirty = false;

        if (entity.IsDeleted)
        {
            await _local.DeletePermanentAsync(entity);
            return;
        }

        await _local.UpdateAsync(entity);
    }

    // ==============================================================
    // DOWNLOAD
    // ==============================================================

    private async Task DownloadUpdatesAsync()
    {
        _logger.LogInformation("⬇ DownloadUpdatesAsync: buscando atualizações do servidor...");

        await DownloadAccountsAsync();
        await DownloadTransactionsAsync();
        await DownloadCardsAsync();
        await DownloadInvoicesAsync();
    }

    // ---------------------- ACCOUNTS ------------------------------

    private async Task DownloadAccountsAsync()
    {
        try
        {
            var server = await _api.GetAccountsForSyncAsync();
            var local = await _local.GetAccountsAsync();

            foreach (var dto in server)
            {
                var exists = local.FirstOrDefault(x => x.Id == dto.Id);

                if (dto.IsDeleted)
                {
                    if (exists != null && !exists.IsDirty)
                        await _local.DeletePermanentAsync(exists);
                    continue;
                }

                if (exists == null)
                    await _local.SaveAccountAsync(ToLocal(dto));
                else if (!exists.IsDirty && dto.UpdatedAt > exists.UpdatedAt)
                    await _local.SaveAccountAsync(ToLocal(dto));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro no download de contas.");
        }
    }

    // ---------------------- TRANSACTIONS --------------------------

    private async Task DownloadTransactionsAsync()
    {
        try
        {
            var server = await _api.GetTransactionsForSyncAsync();
            var local = await _local.GetTransactionsAsync();

            foreach (var dto in server)
            {
                var exists = local.FirstOrDefault(x => x.Id == dto.Id);

                if (dto.IsDeleted)
                {
                    if (exists != null && !exists.IsDirty)
                        await _local.DeletePermanentAsync(exists);
                    continue;
                }

                if (exists == null)
                    await _local.SaveTransactionAsync(TransactionLocalMapper.ToLocal(dto));
                else if (!exists.IsDirty && dto.UpdatedAt > exists.UpdatedAt)
                    await _local.SaveTransactionAsync(TransactionLocalMapper.ToLocal(dto));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro no download de transações.");
        }
    }

    // ---------------------- CARDS ------------------------------

    private async Task DownloadCardsAsync()
    {
        try
        {
            var server = await _api.GetCardsForSyncAsync();
            var local = await _local.GetCreditCardsAsync();

            foreach (var dto in server)
            {
                var exists = local.FirstOrDefault(x => x.Id == dto.Id);

                if (dto.IsDeleted)
                {
                    if (exists != null && !exists.IsDirty)
                        await _local.DeletePermanentAsync(exists);
                    continue;
                }

                if (exists == null)
                    await _local.SaveCreditCardAsync(ToLocal(dto));
                else if (!exists.IsDirty && dto.UpdatedAt > exists.UpdatedAt)
                    await _local.SaveCreditCardAsync(ToLocal(dto));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro no download de cartões.");
        }
    }

    // ---------------------- INVOICES ------------------------------

    private async Task DownloadInvoicesAsync()
    {
        try
        {
            var server = await _api.GetInvoicesForSyncAsync();
            var local = await _local.GetInvoicesAsync();

            foreach (var dto in server)
            {
                var exists = local.FirstOrDefault(x => x.Id == dto.Id);

                if (dto.IsDeleted)
                {
                    if (exists != null && !exists.IsDirty)
                        await _local.DeletePermanentAsync(exists);
                    continue;
                }

                if (exists == null)
                    await _local.SaveInvoiceAsync(ToLocal(dto));
                else if (!exists.IsDirty && dto.UpdatedAt > exists.UpdatedAt)
                    await _local.SaveInvoiceAsync(ToLocal(dto));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro no download de faturas.");
        }
    }

    // ==============================================================
    // MAPPERS
    // ==============================================================

    private AccountDto ToDto(AccountLocal a) => new()
    {
        Id = a.Id,
        Name = a.Name,
        AccountType = (int)a.AccountType,
        Balance = a.Balance,
        Currency = a.Currency,
        UpdatedAt = a.UpdatedAt,
        CreatedAt = a.CreatedAt,
        IsDeleted = a.IsDeleted
    };

    private CreditCardDto ToDto(CreditCardLocal c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Last4Digits = c.Last4Digits,
        CreditLimit = c.CreditLimit,
        CurrentInvoiceId = c.CurrentInvoiceId,
        DueDay = c.DueDay,
        ClosingDay = c.ClosingDay,
        UpdatedAt = c.UpdatedAt,
        CreatedAt = c.CreatedAt,
        IsDeleted = c.IsDeleted
    };

    private InvoiceDto ToDto(InvoiceLocal i) => new()
    {
        Id = i.Id,
        CreditCardId = i.CreditCardId,
        Month = i.Month,
        Year = i.Year,
        Total = i.Total,
        PaidAmount = i.PaidAmount,
        ClosingDate = i.ClosingDate,
        DueDate = i.DueDate,
        IsPaid = i.IsPaid,
        UpdatedAt = i.UpdatedAt,
        CreatedAt = i.CreatedAt,
        IsDeleted = i.IsDeleted
    };

    private AccountLocal ToLocal(AccountDto d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Balance = d.Balance,
        Currency = d.Currency,
        AccountType = (AccountType)d.AccountType,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt,
        IsDeleted = d.IsDeleted
    };

    private CreditCardLocal ToLocal(CreditCardDto d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Last4Digits = d.Last4Digits,
        CreditLimit = d.CreditLimit,
        CurrentInvoiceId = d.CurrentInvoiceId ?? Guid.Empty,
        ClosingDay = d.ClosingDay,
        DueDay = d.DueDay,
        IsDeleted = d.IsDeleted,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt
    };

    private InvoiceLocal ToLocal(InvoiceDto d) => new()
    {
        Id = d.Id,
        CreditCardId = d.CreditCardId,
        Month = d.Month,
        Year = d.Year,
        Total = d.Total,
        PaidAmount = d.PaidAmount,
        ClosingDate = d.ClosingDate,
        DueDate = d.DueDate,
        IsPaid = d.IsPaid,
        IsDeleted = d.IsDeleted,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt
    };
}
