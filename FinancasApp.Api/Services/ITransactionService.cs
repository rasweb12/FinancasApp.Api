using FinancasApp.Api.Data;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancasApp.Api.Services;

public interface ITransactionService
{
    Task SyncAsync(List<TransactionDto> items, Guid userId);
    Task<List<Transaction>> GetAllAsync(Guid userId);
}

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _db;

    public TransactionService(AppDbContext db)
    {
        _db = db;
    }

    // =====================================================================
    // 📌 LISTAR TODAS (PARA RETORNO DO SYNC)
    // =====================================================================
    public async Task<List<Transaction>> GetAllAsync(Guid userId)
    {
        return await _db.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    // =====================================================================
    // 🔄 SYNC COMPLETO
    // =====================================================================
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

    // =====================================================================
    // 🔧 CRIAR OU ATUALIZAR (UP/SERT)
    // =====================================================================
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
        else
        {
            ApplyUpdates(existing, dto);
        }
    }

    // =====================================================================
    // ❌ DELETE
    // =====================================================================
    private async Task DeleteAsync(Guid id, Guid userId)
    {
        var existing = await _db.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (existing != null)
        {
            _db.Transactions.Remove(existing);
        }
    }

    // =====================================================================
    // 💳 PARCELAMENTO
    // =====================================================================
    private async Task SyncInstallmentsAsync(TransactionDto dto, Guid userId)
    {
        Guid groupId = dto.TransactionGroupId ?? Guid.NewGuid();

        for (int i = 1; i <= dto.InstallmentTotal; i++)
        {
            var installmentDate = dto.Date.AddMonths(i - 1);

            var entity = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AccountId = dto.AccountId,
                CategoryId = dto.CategoryId,
                Type = int.Parse(dto.Type),
                SubType = dto.SubType,

                Description = $"{dto.Description} ({i}/{dto.InstallmentTotal})",
                Amount = dto.Amount,
                Date = installmentDate,
                Tags = dto.Tags,

                InstallmentNumber = i,
                InstallmentTotal = dto.InstallmentTotal,
                TransactionGroupId = groupId,
            };

            _db.Transactions.Add(entity);

            await AttachToInvoiceAsync(entity);
        }
    }

    // =====================================================================
    // 🧾 LIGAR TRANSAÇÃO À FATURA CORRESPONDENTE
    // =====================================================================
    private async Task AttachToInvoiceAsync(Transaction trx)
    {
        var card = await _db.CreditCards
            .FirstOrDefaultAsync(c => c.AccountId == trx.AccountId);

        if (card == null)
            return;

        int month = trx.Date.Month;
        int year = trx.Date.Year;

        var invoice = await _db.Invoices
            .FirstOrDefaultAsync(i =>
                i.CreditCardId == card.Id &&
                i.Month == month &&
                i.Year == year
            );

        if (invoice == null)
        {
            invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                CreditCardId = card.Id,
                Month = month,
                Year = year,
                Total = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = trx.UserId.ToString()
            };

            _db.Invoices.Add(invoice);
        }

        invoice.Total += trx.Amount;
        invoice.UpdatedAt = DateTime.UtcNow;
    }

    // =====================================================================
    // 🧩 MAP DTO → ENTITY
    // =====================================================================
    private Transaction MapToEntity(TransactionDto dto, Guid userId)
    {
        return new Transaction
        {
            Id = dto.Id,
            UserId = userId,
            AccountId = dto.AccountId,
            CategoryId = dto.CategoryId,
            Type = int.Parse(dto.Type),
            SubType = dto.SubType,
            Amount = dto.Amount,
            Date = dto.Date,
            Description = dto.Description,
            Tags = dto.Tags,
            InstallmentNumber = dto.InstallmentNumber,
            InstallmentTotal = dto.InstallmentTotal,
            IsRecurring = dto.IsRecurring,
            TransactionGroupId = dto.TransactionGroupId,
            CreatedAt = DateTime.UtcNow
        };
    }

    // =====================================================================
    // 🧩 APLICAR ATUALIZAÇÃO
    // =====================================================================
    private void ApplyUpdates(Transaction entity, TransactionDto dto)
    {
        entity.AccountId = dto.AccountId;
        entity.CategoryId = dto.CategoryId;
        entity.Type = int.Parse(dto.Type);
        entity.SubType = dto.SubType;
        entity.Amount = dto.Amount;
        entity.Date = dto.Date;
        entity.Description = dto.Description;
        entity.Tags = dto.Tags;
        entity.InstallmentNumber = dto.InstallmentNumber;
        entity.InstallmentTotal = dto.InstallmentTotal;
        entity.IsRecurring = dto.IsRecurring;
        entity.TransactionGroupId = dto.TransactionGroupId;
    }
}
