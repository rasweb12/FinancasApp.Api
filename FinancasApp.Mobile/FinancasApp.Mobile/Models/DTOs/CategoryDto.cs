    using System;

    namespace FinancasApp.Mobile.Models.DTOs;

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; } // "Expense" ou "Income"
        public string? Icon { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
    }
