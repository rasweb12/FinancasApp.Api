// Models/Transaction.cs
using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.Models;

public class Transaction : ISyncableEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid? CreditCardId { get; set; }
    public CreditCard? CreditCard { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public int? CategoryId { get; set; }
    public string Category { get; set; } = string.Empty;

    // Tipo da transação: "Income", "Expense", "Transfer"
    public string Type { get; set; } = "Expense";

    // SubType: ex: "Salary", "Food", "Transport", "Investment" → pra relatórios futuros
    public int? SubType { get; set; }

    // Tags como CSV: "supermercado, comida, urgente"
    public string Tags { get; set; } = string.Empty;

    public int? InstallmentNumber { get; set; }
    public int? InstallmentTotal { get; set; }
    public Guid? TransactionGroupId { get; set; }

    public bool IsRecurring { get; set; }

    // === Campos de Sync (ISyncableEntity) ===
    public bool IsNew { get; set; } = true;
    public bool IsDirty { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Relacionamento N:N com Tag
    public List<TransactionTag> TransactionTags { get; set; } = new();
}