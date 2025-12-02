// Services/AccountService.cs
using AutoMapper;
using FinancasApp.Api.Data;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using FinancasApp.Api.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FinancasApp.Api.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AccountService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task SyncAsync(List<AccountDto> accounts, Guid userId)
    {
        foreach (var dto in accounts)
        {
            var entity = await _context.Accounts.FindAsync(dto.Id);

            if (entity == null)
            {
                entity = _mapper.Map<Account>(dto);
                entity.UserId = userId;
                _context.Accounts.Add(entity);
            }
            else if (dto.IsDeleted)
            {
                entity.IsDeleted = true;
                entity.MarkAsDirty();
            }
            else if (dto.UpdatedAt > entity.UpdatedAt)
            {
                _mapper.Map(dto, entity);
                entity.UserId = userId;
                entity.MarkAsDirty();
            }

            if (!dto.IsDeleted && (entity.IsNew || entity.IsDirty))
                entity.MarkAsSynced();
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<Account>> GetAllAsync(Guid userId)
    {
        return await _context.Accounts
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .ToListAsync();
    }
}