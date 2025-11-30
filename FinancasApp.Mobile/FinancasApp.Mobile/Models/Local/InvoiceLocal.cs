using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public class InvoiceLocal : BaseEntity
    {
        public Guid CreditCardId { get; set; }

        public DateTime ClosingDate { get; set; }
        public DateTime DueDate { get; set; }

        public bool IsClosed { get; set; } = false;
        public bool IsPaid { get; set; } = false;

        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
    }
}
