using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public class TagAssignmentLocal : BaseEntity
    {
        public Guid TransactionId { get; set; }

        public Guid TagId { get; set; }

        public bool IsDirty { get; set; } = false;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
