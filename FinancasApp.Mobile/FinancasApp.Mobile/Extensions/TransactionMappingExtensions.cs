// Extensions/TransactionMappingExtensions.cs
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;

// Resolve ambiguidade do enum de uma vez por todas
using TransactionType = FinancasApp.Mobile.Models.Local.TransactionType;

namespace FinancasApp.Mobile.Extensions;

public static class TransactionMappingExtensions
{
    // Local → DTO (usado no Push)
    public static TransactionDto ToDto(this TransactionLocal local)
    {
        return new TransactionDto
        {
            Id = local.Id,
            AccountId = local.AccountId,
            Description = local.Description ?? string.Empty,
            Amount = local.Amount,
            Date = local.Date,
            CategoryId = local.CategoryId,
            Category = local.Category ?? string.Empty,
            Type = local.Type.ToString(), // "Income", "Expense", "Transfer"
            SubType = local.SubType,
            Tags = local.Tags == null || local.Tags.Count == 0
                ? string.Empty
                : string.Join(",", local.Tags),
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

    // DTO → Local (novo registro vindo do servidor)
    public static TransactionLocal ToLocal(this TransactionDto dto)
    {
        var tags = string.IsNullOrWhiteSpace(dto.Tags)
            ? new List<string>()
            : dto.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(t => t.Trim())
                      .Where(t => !string.IsNullOrEmpty(t))
                      .ToList();

        return new TransactionLocal
        {
            Id = dto.Id,
            AccountId = dto.AccountId,
            Description = dto.Description ?? string.Empty,
            Amount = dto.Amount,
            Date = dto.Date,
            CategoryId = dto.CategoryId,
            Category = dto.Category ?? string.Empty,
            Type = Enum.TryParse<TransactionType>(dto.Type, ignoreCase: true, out var type)
                ? type
                : TransactionType.Expense,
            SubType = dto.SubType,
            Tags = tags,
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

    // Atualiza Local com dados do servidor (resolução de conflito)
    public static void UpdateFromDto(this TransactionLocal local, TransactionDto dto)
    {
        local.AccountId = dto.AccountId;
        local.Description = dto.Description ?? string.Empty;
        local.Amount = dto.Amount;
        local.Date = dto.Date;
        local.CategoryId = dto.CategoryId;
        local.Category = dto.Category ?? string.Empty;

        if (Enum.TryParse<TransactionType>(dto.Type, ignoreCase: true, out var type))
            local.Type = type;

        local.SubType = dto.SubType;

        local.Tags = string.IsNullOrWhiteSpace(dto.Tags)
            ? new List<string>()
            : dto.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(t => t.Trim())
                      .Where(t => !string.IsNullOrEmpty(t))
                      .ToList();

        local.InstallmentNumber = dto.InstallmentNumber;
        local.InstallmentTotal = dto.InstallmentTotal;
        local.TransactionGroupId = dto.TransactionGroupId;
        local.IsRecurring = dto.IsRecurring;
        local.IsDeleted = dto.IsDeleted;
        local.UpdatedAt = dto.UpdatedAt;
    }
}