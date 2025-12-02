// Models/User.cs
using FinancasApp.Api.Models;
using System.Collections.Generic;

public class User : ISyncableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }

    // === NAVEGAÇÕES (OBRIGATÓRIAS PARA O EF CORE) ===
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<CreditCard> CreditCards { get; set; } = new List<CreditCard>(); // ← ESSA LINHA FALTAVA
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    // === ISyncableEntity ===
    public bool IsNew { get; set; } = true;
    public bool IsDirty { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}