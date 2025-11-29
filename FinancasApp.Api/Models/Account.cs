using System;
using System.Collections.Generic;

namespace FinancasApp.Api.Models
{
    public class Account : ISyncableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        // usar enum no app — no JSON vira int automaticamente
        public int AccountType { get; set; }
        public string Currency { get; set; } = "BRL";
        public decimal Balance { get; set; }
        public decimal InitialBalance { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }
}
