using FinancasApp.Api.Models;

public class Invoice : ISyncableEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public Guid CreditCardId { get; set; }
    public CreditCard CreditCard { get; set; } = default!;

    public DateTime ClosingDate { get; set; }
    public DateTime DueDate { get; set; }

    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public bool IsNew { get; set; }
    public bool IsPaid { get; set; }
    public bool IsDirty { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
