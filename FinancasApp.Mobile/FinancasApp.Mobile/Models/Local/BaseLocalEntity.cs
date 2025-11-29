using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public abstract class BaseLocalEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // FLAGS DE SINCRONIZAÇÃO
        public bool IsNew { get; set; } = true;
        public bool IsDirty { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public DateTime? SyncDate { get; set; } = null;
        public int RetryCount { get; set; } = 0;
    }
}
