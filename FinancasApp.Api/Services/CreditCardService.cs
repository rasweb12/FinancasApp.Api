// CreditCardService.cs
using AutoMapper;
using FinancasApp.Api.Data;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancasApp.Api.Services;

public class CreditCardService : ICreditCardService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CreditCardService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CreditCardDto>> GetByUserAsync(Guid userId)
    {
        var entities = await _context.CreditCards
            .Where(c => c.UserId == userId && !c.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<CreditCardDto>>(entities);
    }

    public async Task<List<CreditCardDto>> GetAllAsync(Guid userId)
    {
        // Reutiliza GetByUserAsync (mesmo filtro: user + not deleted)
        return await GetByUserAsync(userId);
    }

    public async Task<List<CreditCardDto>> GetForSyncAsync(Guid userId)
    {
        return await GetByUserAsync(userId); // Mesmo filtro
    }

    public async Task<CreditCardDto> CreateAsync(CreditCardDto dto, Guid userId)
    {
        var entity = _mapper.Map<CreditCard>(dto);
        entity.UserId = userId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.CreditCards.Add(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<CreditCardDto>(entity);
    }

    public async Task UpdateAsync(CreditCardDto dto)
    {
        var entity = await _context.CreditCards.FindAsync(dto.Id);
        if (entity == null) throw new KeyNotFoundException();

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var entity = await _context.CreditCards.FindAsync(id);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task SyncAsync(List<CreditCardDto> dtos, Guid userId)
    {
        foreach (var dto in dtos)
        {
            var entity = await _context.CreditCards.FindAsync(dto.Id);

            if (entity == null)
            {
                entity = _mapper.Map<CreditCard>(dto);
                entity.UserId = userId;
                _context.CreditCards.Add(entity);
            }
            else if (dto.IsDeleted)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (dto.UpdatedAt > entity.UpdatedAt)
            {
                _mapper.Map(dto, entity);
                entity.UserId = userId;
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }
}