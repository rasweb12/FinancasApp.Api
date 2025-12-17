using SQLite;
using System;

namespace FinancasApp.Mobile.Models.Local
{
    public enum CategoryType
    {
        Expense,
        Income
    }

    [Table("Categories")]
    public class CategoryLocal : BaseEntity  // ◄ AQUI ESTAVA O PROBLEMA! AGORA CORRIGIDO
    {
        public Guid? UserId { get; set; }

        [Indexed] // Melhora performance em buscas
        public string Name { get; set; } = string.Empty;

        public CategoryType Type { get; set; } = CategoryType.Expense;

        public string Icon { get; set; } = "other.png";

        public bool IsSystem { get; set; } = false;

        public int TransactionCount { get; set; } = 0;

        // Flags de sincronização (herdadas de BaseEntity)
        // public bool IsDirty { get; set; } = true;
        // public bool IsDeleted { get; set; } = false;
        // Não precisa mais declarar
        // public DateTime CreatedAt { get; set; }
        // public DateTime UpdatedAt { get; set; }
    }
}