using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public class InvoiceLocal : BaseEntity
    {
        // RELACIONAMENTO
        public Guid CreditCardId { get; set; }

        // DATAS
        public DateTime ClosingDate { get; set; }
        public DateTime DueDate { get; set; }

        // STATUS
        public bool IsClosed { get; set; } = false;
        public bool IsPaid { get; set; } = false;

        // VALORES
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }

        // IDENTIFICADORES
        public int Month { get; set; }
        public int Year { get; set; }


    }
}
