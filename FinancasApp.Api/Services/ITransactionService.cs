using FinancasApp.Api.Data;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancasApp.Api.Services;

public interface ITransactionService
{
    Task<List<Transaction>> GetAllAsync(Guid userId);
    Task SyncAsync(List<TransactionDto> items, Guid userId);
}

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _db;

    public TransactionService(AppDbContext db) => _db = db;

    public async Task<List<Transaction>> GetAllAsync(Guid userId)
    {
        return await _db.Transactions
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task SyncAsync(List<TransactionDto> items, Guid userId)
    {
        foreach (var dto in items)
        {
            if (dto.IsDeleted)
            {
                await DeleteAsync(dto.Id, userId);
                continue;
            }

            if (dto.InstallmentTotal > 1)
            {
                await SyncInstallmentsAsync(dto, userId);
                continue;
            }

            await UpsertAsync(dto, userId);
        }

        await _db.SaveChangesAsync();
    }

    private async Task UpsertAsync(TransactionDto dto, Guid userId)
    {
        var existing = await _db.Transactions
            .FirstOrDefaultAsync(t => t.Id == dto.Id && t.UserId == userId);

        if (existing == null)
        {
            var entity = MapToEntity(dto, userId);
            _db.Transactions.Add(entity);
            await AttachToInvoiceAsync(entity);
        }
        else if (dto.UpdatedAt > existing.UpdatedAt || existing.IsDeleted)
        {
            ApplyUpdates(existing, dto);
            existing.IsDeleted = false;
            existing.IsDirty = true;
        }

        if (existing != null)
        {
            existing.IsDirty = false; // Synced
        }
    }

    private async Task DeleteAsync(Guid id, Guid userId)
    {
        var entity = await _db.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.IsDirty = true;
        }
    }

    private async Task SyncInstallmentsAsync(TransactionDto dto, Guid userId)
    {
        var groupId = dto.TransactionGroupId ?? Guid.NewGuid();

        for (int i = 1; i <= dto.InstallmentTotal; i++)
        {
            var installmentDate = dto.Date.AddMonths(i - 1);

            var trx = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AccountId = dto.AccountId,
                Description = $"{dto.Description} ({i}/{dto.InstallmentTotal})",
                Amount = dto.Amount,
                Date = installmentDate,
                Type = dto.Type,
                SubType = dto.SubType,
                CategoryId = dto.CategoryId, // Guid?
                Tags = dto.Tags,
                InstallmentNumber = i,
                InstallmentTotal = dto.InstallmentTotal,
                TransactionGroupId = groupId,
                IsRecurring = dto.IsRecurring,
                IsNew = true,
                IsDirty = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Transactions.Add(trx);
            await AttachToInvoiceAsync(trx);
        }
    }

    private async Task AttachToInvoiceAsync(Transaction trx)
    {
        // Lógica desativada temporariamente
        return;
    }

    private Transaction MapToEntity(TransactionDto dto, Guid userId)
    {
        return new Transaction
        {
            Id = dto.Id,
            UserId = userId,
            AccountId = dto.AccountId,
            Description = dto.Description,
            Amount = dto.Amount,
            Date = dto.Date,
            Type = dto.Type,
            SubType = dto.SubType,
            CategoryId = dto.CategoryId,
            Tags = dto.Tags,
            InstallmentNumber = dto.InstallmentNumber,
            InstallmentTotal = dto.InstallmentTotal,
            TransactionGroupId = dto.TransactionGroupId,
            IsRecurring = dto.IsRecurring,
            IsNew = dto.IsNew,
            IsDirty = true,
            IsDeleted = false,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private void ApplyUpdates(Transaction entity, TransactionDto dto)
    {
        entity.AccountId = dto.AccountId;
        entity.Description = dto.Description;
        entity.Amount = dto.Amount;
        entity.Date = dto.Date;
        entity.Type = dto.Type;
        entity.SubType = dto.SubType;
        entity.CategoryId = dto.CategoryId;
        entity.Tags = dto.Tags;
        entity.InstallmentNumber = dto.InstallmentNumber;
        entity.InstallmentTotal = dto.InstallmentTotal;
        entity.TransactionGroupId = dto.TransactionGroupId;
        entity.IsRecurring = dto.IsRecurring;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}