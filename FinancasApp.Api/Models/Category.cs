using System;
using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.Models
{
    public class Category
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSystem { get; set; }
        // ← NAVEGAÇÃO PARA O OUTRO LADO
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}