using System;

namespace FinancasApp.Api.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Expense";
        public string Icon { get; set; } = "other.png";
        public bool IsSystem { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TransactionCount { get; set; } = 0;
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Expense";
        public string Icon { get; set; } = "other.png";
    }
}