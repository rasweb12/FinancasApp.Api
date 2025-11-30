using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Extensions;

public static class TransactionMappingExtensions
{
    // -----------------------------
    // LOCAL → DTO (PUSH)
    // -----------------------------
    public static TransactionDto ToDto(this TransactionLocal local)
    {
        return new TransactionDto
        {
            Id = local.Id,
            AccountId = local.AccountId,
            Description = local.Description ?? string.Empty,
            Amount = local.Amount,
            Date = local.Date,

            // int? → Guid? não é possível. Mantemos null.
            CategoryId = null,
            Category = local.Category ?? string.Empty,

            // string → string
            Type = local.Type ?? string.Empty,

            // int? → string?
            SubType = local.SubType?.ToString(),

            // string → string
            Tags = local.Tags ?? string.Empty,

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

    // -----------------------------
    // DTO → NOVO LOCAL (PULL)
    // -----------------------------
    public static TransactionLocal ToLocal(this TransactionDto dto)
    {
        return new TransactionLocal
        {
            Id = dto.Id,
            AccountId = dto.AccountId,

            Description = dto.Description ?? string.Empty,
            Amount = dto.Amount,
            Date = dto.Date,

            // Guid? → int? é impossível → armazenamos null
            CategoryId = null,
            Category = dto.Category ?? string.Empty,

            // string → string
            Type = dto.Type ?? string.Empty,

            // string? → int?
            SubType = int.TryParse(dto.SubType, out var st) ? st : null,

            // string → string
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

    // -----------------------------
    // DTO → ATUALIZAR LOCAL EXISTENTE (CONFLITO)
    // -----------------------------
    public static void UpdateFromDto(this TransactionLocal local, TransactionDto dto)
    {
        local.AccountId = dto.AccountId;
        local.Description = dto.Description ?? string.Empty;
        local.Amount = dto.Amount;
        local.Date = dto.Date;

        local.CategoryId = null;  // por consistência
        local.Category = dto.Category ?? string.Empty;

        local.Type = dto.Type ?? string.Empty;

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
