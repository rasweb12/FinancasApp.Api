// Models/DTOs/TransactionDto.cs
using System;

namespace FinancasApp.Mobile.Models.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }

    public int? CategoryId { get; set; }  // ← mude no DTO
    public string Category { get; set; } = string.Empty;

    public string Type { get; set; } = "Expense";  // ← string
    public string? SubType { get; set; }           // ← string

    public string Tags { get; set; } = string.Empty; // ← string (CSV)

    public int? InstallmentNumber { get; set; }
    public int? InstallmentTotal { get; set; }
    public Guid? TransactionGroupId { get; set; }  // ← Guid?, NÃO int?

    public bool IsRecurring { get; set; }
    public bool IsNew { get; set; }
    public bool IsDirty { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}