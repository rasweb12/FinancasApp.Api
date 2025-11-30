using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Mappers;

public static class CreditCardMapper
{
    public static CreditCardDto ToDto(CreditCardLocal local) =>
        new()
        {
            Id = local.Id,
            Name = local.Name,
            Last4Digits = local.Last4Digits,
            CreditLimit = local.CreditLimit,
            DueDay = local.DueDay,
            ClosingDay = local.ClosingDay,
            IsDeleted = local.IsDeleted,
            UpdatedAt = local.UpdatedAt
        };

    public static CreditCardLocal ToLocal(CreditCardDto dto) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Last4Digits = dto.Last4Digits,
            CreditLimit = dto.CreditLimit,
            DueDay = dto.DueDay,
            ClosingDay = dto.ClosingDay,
            IsDeleted = dto.IsDeleted,
            UpdatedAt = dto.UpdatedAt,
            CreatedAt = dto.UpdatedAt,
            IsDirty = false
        };
}
