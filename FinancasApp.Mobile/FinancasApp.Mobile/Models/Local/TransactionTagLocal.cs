using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public class TransactionTagLocal : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Icon { get; set; }

        public string? Color { get; set; }

        // Dados vindos da API (se necessário)
        public Guid? TransactionTagId { get; set; }
        public string? TransactionTagName { get; set; }
        public string? TransactionTagColor { get; set; }
        public string? TransactionTagIcon { get; set; }
    }
}
