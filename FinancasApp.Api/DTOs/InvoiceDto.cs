using FinancasApp.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancasApp.Api.DTOs;

public class InvoiceDto : ISyncableDto
{
    public Guid Id { get; set; }
    public Guid CreditCardId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
    public DateTime ClosingDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Total { get; set; } // ✔ precisa existir
    public decimal PaidAmount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IsNew { get; set; }
    public bool IsPaid { get; set; }
    public bool IsDirty { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}

