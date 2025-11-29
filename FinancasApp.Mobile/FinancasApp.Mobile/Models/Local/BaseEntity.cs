using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public abstract class BaseEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        // Controle de sincronização
        public bool IsDirty { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
