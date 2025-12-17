using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToDto(CategoryLocal local)
    {
        return new CategoryDto
        {
            Id = local.Id,
            Name = local.Name,
            Type = local.Type.ToString(),
            Icon = local.Icon,
            IsDeleted = local.IsDeleted,
            UpdatedAt = local.UpdatedAt
        };
    }

    public static CategoryLocal ToLocal(CategoryDto dto)
    {
        return new CategoryLocal
        {
            Id = dto.Id,
            Name = dto.Name,
            Type = Enum.Parse<CategoryType>(dto.Type ?? "Expense"),
            Icon = dto.Icon ?? "other.png",
            IsDeleted = dto.IsDeleted,
            UpdatedAt = dto.UpdatedAt,
            IsDirty = false // Após sync
        };
    }
}