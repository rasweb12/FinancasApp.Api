using System;
using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        // Navegações
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    }
}