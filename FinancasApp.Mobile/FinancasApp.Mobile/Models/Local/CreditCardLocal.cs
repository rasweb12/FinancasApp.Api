using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public class CreditCardLocal : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Last4Digits { get; set; } = string.Empty;
        public decimal CreditLimit { get; set; }

        public Guid? CurrentInvoiceId { get; set; }

        public int DueDay { get; set; }
        public int ClosingDay { get; set; }
    }
}
