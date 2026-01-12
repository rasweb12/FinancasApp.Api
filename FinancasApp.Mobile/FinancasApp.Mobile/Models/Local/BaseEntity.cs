// Models/Local/BaseEntity.cs — MANTENHA EXATAMENTE ASSIM
using SQLite;

namespace FinancasApp.Mobile.Models.Local
{
    public abstract class BaseEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [Ignore] // Não salva no SQLite
        public bool IsDirty { get; set; } = true;  // Marca para sync

        [Ignore]
        public bool IsDeleted { get; set; } = false;

        [Ignore]
        public bool IsNew { get; set; } = true;        // ◄ NOVO: indica que nunca foi sincronizado

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public interface ISyncableEntity
    {
        bool IsDirty { get; set; }
        bool IsDeleted { get; set; }
        bool IsNew { get; set; }          // ◄ Adicionado aqui também
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}