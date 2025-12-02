// Models/CreditCard.cs
using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.Models;

public class CreditCard : ISyncableEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;
    public string Last4Digits { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }

    public int DueDay { get; set; }      // Dia do vencimento (ex: 10)
    public int ClosingDay { get; set; }  // Dia do fechamento (ex: 3)

    // Fatura atual (opcional — aponta pra Invoice atual)
    public Guid? CurrentInvoiceId { get; set; }
    public Invoice? CurrentInvoice { get; set; }

    // === Relacionamentos ===
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public List<Invoice> Invoices { get; set; } = new();

    // === Campos de Sync (ISyncableEntity) ===
    public bool IsNew { get; set; } = true;
    public bool IsDirty { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}