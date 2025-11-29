using System;
using System.Collections.Generic;

namespace FinancasApp.Api.Models
{
    public class Transaction : ISyncableEntity
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public int? CategoryId { get; set; }
        public string Type { get; set; }
        public int? SubType { get; set; }          // novo
        public string Tags { get; set; } = "";     // string CSV de tags
        public int? InstallmentNumber { get; set; }
        public int? InstallmentTotal { get; set; }
        public Guid? TransactionGroupId { get; set; }
        public bool IsRecurring { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<TransactionTag> TransactionTags { get; set; } = new List<TransactionTag>();
    }
}
