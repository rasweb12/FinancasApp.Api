using FinancasApp.Api.Data;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancasApp.Api.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _db;

        public InvoiceService(AppDbContext db)
        {
            _db = db;
        }

        // ----------------------------------------------------------------------
        public async Task<List<Invoice>> GetAllAsync(Guid userId)
        {
            return await _db.Invoices
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        // ----------------------------------------------------------------------
        public async Task<Invoice?> GetByIdAsync(Guid id, Guid userId)
        {
            return await _db.Invoices
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
        }

        // ----------------------------------------------------------------------
        public async Task<Invoice> AddAsync(Invoice invoice)
        {
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
            return invoice;
        }

        // ----------------------------------------------------------------------
        public async Task<Invoice> UpdateAsync(Invoice invoice)
        {
            _db.Invoices.Update(invoice);
            await _db.SaveChangesAsync();
            return invoice;
        }

        // ----------------------------------------------------------------------
        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var existing = await GetByIdAsync(id, userId);
            if (existing == null)
                return false;

            _db.Invoices.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        // ======================================================================
        //                              UPSERT
        // ======================================================================
        public async Task<bool> UpsertAsync(InvoiceDto dto, Guid userId)
        {
            Invoice? existing = null;

            if (dto.Id != Guid.Empty)
                existing = await GetByIdAsync(dto.Id, userId);

            if (existing == null)
            {
                // CREATE
                var newInvoice = new Invoice
                {
                    Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                    UserId = userId,
                    CreditCardId = dto.CreditCardId,
                    ClosingDate = dto.ClosingDate,
                    DueDate = dto.DueDate,
                    Total = dto.Total,
                    PaidAmount = dto.PaidAmount,
                    Month = dto.Month,
                    Year = dto.Year,
                    IsPaid = dto.IsPaid,
                    IsNew = dto.IsNew,
                    IsDirty = dto.IsDirty,
                    IsDeleted = dto.IsDeleted,
                    CreatedAt = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt,
                    UpdatedAt = dto.UpdatedAt == default ? DateTime.UtcNow : dto.UpdatedAt
                };

                await AddAsync(newInvoice);
            }
            else
            {
                // UPDATE
                existing.CreditCardId = dto.CreditCardId;
                existing.ClosingDate = dto.ClosingDate;
                existing.DueDate = dto.DueDate;
                existing.Total = dto.Total;
                existing.PaidAmount = dto.PaidAmount;
                existing.Month = dto.Month;
                existing.Year = dto.Year;
                existing.IsPaid = dto.IsPaid;
                existing.IsNew = dto.IsNew;
                existing.IsDirty = dto.IsDirty;
                existing.IsDeleted = dto.IsDeleted;
                existing.UpdatedAt = DateTime.UtcNow;

                await UpdateAsync(existing);
            }

            return true;
        }

        // ======================================================================
        //                              SYNC
        // ======================================================================
        public async Task SyncAsync(List<InvoiceDto> incoming, Guid userId)
        {
            var existing = await _db.Invoices
                .Where(i => i.UserId == userId)
                .ToListAsync();

            foreach (var dto in incoming)
            {
                var match = existing.FirstOrDefault(x => x.Id == dto.Id);

                if (dto.IsDeleted)
                {
                    if (match != null)
                        _db.Invoices.Remove(match);

                    continue;
                }

                if (match == null)
                {
                    // CREATE
                    _db.Invoices.Add(new Invoice
                    {
                        Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                        UserId = userId,
                        CreditCardId = dto.CreditCardId,
                        ClosingDate = dto.ClosingDate,
                        DueDate = dto.DueDate,
                        Total = dto.Total,
                        PaidAmount = dto.PaidAmount,
                        Month = dto.Month,
                        Year = dto.Year,
                        IsPaid = dto.IsPaid,
                        IsNew = dto.IsNew,
                        IsDirty = dto.IsDirty,
                        IsDeleted = dto.IsDeleted,
                        CreatedAt = dto.CreatedAt,
                        UpdatedAt = dto.UpdatedAt
                    });
                }
                else
                {
                    // UPDATE
                    match.CreditCardId = dto.CreditCardId;
                    match.ClosingDate = dto.ClosingDate;
                    match.DueDate = dto.DueDate;
                    match.Total = dto.Total;
                    match.PaidAmount = dto.PaidAmount;
                    match.Month = dto.Month;
                    match.Year = dto.Year;
                    match.IsPaid = dto.IsPaid;
                    match.IsNew = dto.IsNew;
                    match.IsDirty = dto.IsDirty;
                    match.IsDeleted = dto.IsDeleted;
                    match.UpdatedAt = dto.UpdatedAt;
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
