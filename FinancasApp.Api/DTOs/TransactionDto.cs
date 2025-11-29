using System;

namespace FinancasApp.Api.DTOs
{
    public class TransactionDto : ISyncableDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public int? CategoryId { get; set; }   // ALTERADO

        public string Type { get; set; }
        public int? SubType { get; set; }
        public string Tags { get; set; } = "";
        public int? InstallmentNumber { get; set; }
        public int? InstallmentTotal { get; set; }
        public Guid? TransactionGroupId { get; set; }
        public bool IsRecurring { get; set; }
        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public enum TransactionType { Income = 1, Expense = 2, Transfer = 3 }
}
