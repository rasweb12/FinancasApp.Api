using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Mappers;

public static class AccountMapper
{
    public static AccountDto ToDto(AccountLocal local) =>
        new()
        {
            Id = local.Id,
            Name = local.Name,
            Balance = local.Balance,
            IsDeleted = local.IsDeleted,
            UpdatedAt = local.UpdatedAt
        };

    public static AccountLocal ToLocal(AccountDto dto) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Balance = dto.Balance,
            IsDeleted = dto.IsDeleted,
            UpdatedAt = dto.UpdatedAt,
            CreatedAt = dto.UpdatedAt,
            IsDirty = false
        };
}
