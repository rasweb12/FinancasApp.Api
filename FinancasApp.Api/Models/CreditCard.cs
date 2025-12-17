using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.Models;

public class CreditCard : ISyncableEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(4)]
    public string Last4Digits { get; set; } = "0000";

    public decimal CreditLimit { get; set; }

    public int DueDay { get; set; } // 1-31

    public int ClosingDay { get; set; } // Calculado

    public Guid? CurrentInvoiceId { get; set; }
    public Invoice? CurrentInvoice { get; set; }

    // Relacionamentos
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public List<Invoice> Invoices { get; set; } = new();

    // Sync flags
    public bool IsNew { get; set; } = true;
    public bool IsDirty { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}