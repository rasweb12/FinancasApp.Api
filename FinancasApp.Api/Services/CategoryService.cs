// CategoryService.cs
using AutoMapper;
using FinancasApp.Api.Data;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancasApp.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> GetByUserAsync(Guid userId)
    {
        var entities = await _context.Categories
            .Where(c => (c.UserId == userId || c.IsSystem) && !c.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<CategoryDto>>(entities);
    }
    public async Task<List<CategoryDto>> GetAllAsync(Guid userId)
    {
        // Mesmo filtro que GetByUserAsync
        return await GetByUserAsync(userId);
    }
    public async Task<List<CategoryDto>> GetForSyncAsync(Guid userId)
    {
        var entities = await _context.Categories
            .Where(c => (c.UserId == userId || c.IsSystem) && !c.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<CategoryDto>>(entities);
    }

    public async Task<CategoryDto> CreateAsync(CategoryDto dto, Guid userId)
    {
        var entity = _mapper.Map<Category>(dto);
        entity.UserId = dto.IsSystem ? null : userId;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryDto>(entity);
    }

    public async Task UpdateAsync(CategoryDto dto)
    {
        var entity = await _context.Categories.FindAsync(dto.Id);
        if (entity == null) throw new KeyNotFoundException();

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var entity = await _context.Categories.FindAsync(id);
        if (entity == null) return;

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task SyncAsync(List<CategoryDto> dtos, Guid userId)
    {
        foreach (var dto in dtos)
        {
            var entity = await _context.Categories.FindAsync(dto.Id);

            if (entity == null)
            {
                entity = _mapper.Map<Category>(dto);
                entity.UserId = dto.IsSystem ? null : userId;
                _context.Categories.Add(entity);
            }
            else if (dto.IsDeleted)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (dto.UpdatedAt > entity.UpdatedAt)
            {
                _mapper.Map(dto, entity);
                entity.UserId = dto.IsSystem ? null : userId;
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
    }
}