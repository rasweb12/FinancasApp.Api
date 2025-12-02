using FinancasApp.Api.Data;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancasApp.Api.Services;

public class CreditCardService : ICreditCardService
{
    private readonly AppDbContext _db;

    public CreditCardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CreditCard>> GetAllAsync(Guid userId)
    {
        return await _db.CreditCards
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<CreditCard?> GetByIdAsync(Guid id, Guid userId)
    {
        return await _db.CreditCards
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
    }

    public async Task<CreditCard> AddAsync(CreditCard card)
    {
        _db.CreditCards.Add(card);
        await _db.SaveChangesAsync();
        return card;
    }

    public async Task<CreditCard> UpdateAsync(CreditCard card)
    {
        _db.CreditCards.Update(card);
        await _db.SaveChangesAsync();
        return card;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var card = await GetByIdAsync(id, userId);
        if (card == null) return false;

        _db.CreditCards.Remove(card);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task SyncAsync(List<CreditCardDto> incoming, Guid userId)
    {
        foreach (var dto in incoming)
        {
            var existing = await _db.CreditCards
                .FirstOrDefaultAsync(x => x.Id == dto.Id && x.UserId == userId);

            if (dto.IsDeleted)
            {
                if (existing != null)
                    _db.CreditCards.Remove(existing);

                continue;
            }

            if (existing == null)
            {
                var card = new CreditCard
                {
                    Id = dto.Id,
                    UserId = userId,
                    Name = dto.Name,
                    CreditLimit = dto.CreditLimit,
                    ClosingDay = dto.ClosingDay,
                    DueDay = dto.DueDay,
                    CreatedAt = DateTime.UtcNow
                };

                _db.CreditCards.Add(card);
            }
            else
            {
                existing.Name = dto.Name;
                existing.CreditLimit = dto.CreditLimit;
                existing.ClosingDay = dto.ClosingDay;
                existing.DueDay = dto.DueDay;
                existing.UpdatedAt = DateTime.UtcNow;

                _db.CreditCards.Update(existing);
            }
        }

        await _db.SaveChangesAsync();
    }
}
