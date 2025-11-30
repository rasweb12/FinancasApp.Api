// Extensions/TransactionMappingExtensions.cs
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Extensions;

public static class TransactionMappingExtensions
{
    public static TransactionDto ToDto(this TransactionLocal local)
    {
        return new TransactionDto
        {
            Id = local.Id,
            AccountId = local.AccountId,
            Description = local.Description ?? string.Empty,
            Amount = local.Amount,
            Date = local.Date,
            CategoryId = local.CategoryId,  // int? → int?
            Category = local.Category ?? string.Empty,
            Type = local.Type ?? "Expense",                    // string → string
            SubType = local.SubType?.ToString(),
            Tags = local.Tags ?? string.Empty,                 // string → string
            InstallmentNumber = local.InstallmentNumber,
            InstallmentTotal = local.InstallmentTotal,
            TransactionGroupId = local.TransactionGroupId,
            IsRecurring = local.IsRecurring,
            IsNew = local.Id == Guid.Empty,
            IsDirty = local.IsDirty,
            IsDeleted = local.IsDeleted,
            CreatedAt = local.CreatedAt,
            UpdatedAt = local.UpdatedAt
        };
    }

    public static TransactionLocal ToLocal(this TransactionDto dto)
    {
        return new TransactionLocal
        {
            Id = dto.Id,
            AccountId = dto.AccountId,
            Description = dto.Description ?? string.Empty,
            Amount = dto.Amount,
            Date = dto.Date,
            CategoryId = dto.CategoryId != null ? 1 : null, // ajuste conforme sua lógica real de CategoryId
            Category = dto.Category ?? string.Empty,
            Type = string.IsNullOrWhiteSpace(dto.Type) ? "Expense" : dto.Type,
            SubType = int.TryParse(dto.SubType, out var st) ? st : null,
            Tags = dto.Tags ?? string.Empty,
            InstallmentNumber = dto.InstallmentNumber,
            InstallmentTotal = dto.InstallmentTotal,
            TransactionGroupId = dto.TransactionGroupId,
            IsRecurring = dto.IsRecurring,
            IsDirty = false,
            IsDeleted = dto.IsDeleted,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }

    public static void UpdateFromDto(this TransactionLocal local, TransactionDto dto)
    {
        local.AccountId = dto.AccountId;
        local.Description = dto.Description ?? string.Empty;
        local.Amount = dto.Amount;
        local.Date = dto.Date;
        local.Category = dto.Category ?? string.Empty;
        local.Type = string.IsNullOrWhiteSpace(dto.Type) ? "Expense" : dto.Type;
        local.SubType = int.TryParse(dto.SubType, out var st) ? st : null;
        local.Tags = dto.Tags ?? string.Empty;
        local.InstallmentNumber = dto.InstallmentNumber;
        local.InstallmentTotal = dto.InstallmentTotal;
        local.TransactionGroupId = dto.TransactionGroupId;
        local.IsRecurring = dto.IsRecurring;
        local.IsDeleted = dto.IsDeleted;
        local.UpdatedAt = dto.UpdatedAt;
    }
}