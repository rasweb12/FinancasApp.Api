// Models/Local/BaseEntity.cs — MANTENHA EXATAMENTE ASSIM
using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public abstract class BaseEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public bool IsDirty { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}