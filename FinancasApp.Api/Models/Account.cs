using FinancasApp.Api.DTOs;
using System;
using System.Collections.Generic;

namespace FinancasApp.Api.Models
{
    public class Account : ISyncableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // usar enum no app — no JSON vira int automaticamente
        public AccountTypeDto AccountType { get; set; } = AccountTypeDto.Checking;
        public string Currency { get; set; } = "BRL";
        public decimal Balance { get; set; }
        public decimal InitialBalance { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public List<Transaction> Transactions { get; set; } = new();
    }
}
