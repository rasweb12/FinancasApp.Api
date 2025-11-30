using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public class TransactionLocal : BaseEntity
    {
        public Guid AccountId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public int? CategoryId { get; set; }
        public string Category { get; set; } = string.Empty;

        // Type é string → "Income", "Expense", "Transfer"
        public string Type { get; set; } = string.Empty;

        public int? SubType { get; set; }

        // Tags → CSV: "food,market,home"
        public string Tags { get; set; } = string.Empty;

        public int? InstallmentNumber { get; set; }
        public int? InstallmentTotal { get; set; }

        public Guid? TransactionGroupId { get; set; }

        public bool IsRecurring { get; set; }
    }
}
