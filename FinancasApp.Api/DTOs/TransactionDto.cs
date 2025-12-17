using System;
using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.DTOs
{
    public class TransactionDto : ISyncableDto
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        // ◄ CORRIGIDO: Guid? para compatibilidade com Category.Id (Guid)
        public Guid? CategoryId { get; set; }

        // Nome da categoria para exibição no mobile (preenchido no download)
        public string Category { get; set; } = string.Empty;

        public string Type { get; set; } = "Expense"; // "Expense", "Income", "Transfer"

        public int? SubType { get; set; }

        public string Tags { get; set; } = string.Empty;

        public int? InstallmentNumber { get; set; }
        public int? InstallmentTotal { get; set; }

        public Guid? TransactionGroupId { get; set; }

        public bool IsRecurring { get; set; }

        // Sync flags
        public bool IsNew { get; set; } = true;
        public bool IsDirty { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // Enum opcional (se quiser usar no DTO)
    public enum TransactionTypeDto
    {
        Income = 1,
        Expense = 2,
        Transfer = 3
    }
}