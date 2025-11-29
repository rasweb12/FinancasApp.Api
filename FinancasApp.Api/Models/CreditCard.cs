using System;
using System.Collections.Generic;

namespace FinancasApp.Api.Models
{
    public class CreditCard : ISyncableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Last4Digits { get; set; } = "";
        public decimal CreditLimit { get; set; }
        public Guid? CurrentInvoiceId { get; set; }
        public int DueDay { get; set; }
        public int ClosingDay { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Relacionamento
        public List<Invoice> Invoices { get; set; } = new();
    }
}
