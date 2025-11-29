using System;
using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
    }
}