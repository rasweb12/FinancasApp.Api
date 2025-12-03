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

    // === RELACIONAMENTO COM CATEGORIA (CORRETO) ===
    public int? CategoryId { get; set; }
    public Category? Category { get; set; } = null!;  // ← ADICIONADO (obrigatório pro EF)

    // Remova esta linha! Ela que estava quebrando tudo:
    // public string Category { get; set; } = string.Empty;  ← DELETE ESSA PORRA

    public string Type { get; set; } = "Expense";
    public int? SubType { get; set; }
    public string Tags { get; set; } = string.Empty;

    public int? InstallmentNumber { get; set; }
    public int? InstallmentTotal { get; set; }
    public Guid? TransactionGroupId { get; set; }
    public bool IsRecurring { get; set; }

    // Sync fields
    public bool IsNew { get; set; } = true;
    public bool IsDirty { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<TransactionTag> TransactionTags { get; set; } = new();
}