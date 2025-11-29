using FinancasApp.Api.Data;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancasApp.Api.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _db;

        public AccountService(AppDbContext db)
        {
            _db = db;
        }

        // ============================================================
        // 🔄 SYNC
        // ============================================================
        public async Task SyncAsync(List<AccountDto> accounts, Guid userId)
        {
            foreach (var dto in accounts)
            {
                if (dto.IsDeleted)
                {
                    await DeleteAsync(dto.Id, userId);
                    continue;
                }

                if (dto.IsNew)
                {
                    await AddAsync(dto, userId);
                    continue;
                }

                if (dto.IsDirty)
                {
                    await UpdateAsync(dto, userId);
                    continue;
                }
            }

            await _db.SaveChangesAsync();
        }

        // ============================================================
        // 📌 LISTAR TODAS
        // ============================================================
        public async Task<List<Account>> GetAllAsync(Guid userId)
        {
            return await _db.Accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        // ============================================================
        // ➕ ADD
        // ============================================================
        private async Task AddAsync(AccountDto dto, Guid userId)
        {
            var entity = new Account
            {
                Id = dto.Id,
                Name = dto.Name,
                Balance = dto.Balance,
                Type = dto.Type,
                Currency = "BRL",
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Accounts.Add(entity);
        }

        // ============================================================
        // ✏ UPDATE
        // ============================================================
        private async Task UpdateAsync(AccountDto dto, Guid userId)
        {
            var entity = await _db.Accounts
                .FirstOrDefaultAsync(a => a.Id == dto.Id && a.UserId == userId);

            if (entity == null)
                return;

            entity.Name = dto.Name;
            entity.Balance = dto.Balance;
            entity.Type = dto.Type;
        }

        // ============================================================
        // ❌ DELETE
        // ============================================================
        private async Task DeleteAsync(Guid id, Guid userId)
        {
            var entity = await _db.Accounts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (entity != null)
                _db.Accounts.Remove(entity);
        }
    }
}
