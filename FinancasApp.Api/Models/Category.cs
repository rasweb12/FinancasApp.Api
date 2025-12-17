using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.Models
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); // ◄ GUID PARA COMPATIBILIDADE MOBILE

        public Guid? UserId { get; set; } // Null para system

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = "Expense"; // "Expense" ou "Income"

        public string Icon { get; set; } = "other.png";

        public bool IsSystem { get; set; } = false;

        public bool IsDeleted { get; set; } = false; // Soft delete

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navegação
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}