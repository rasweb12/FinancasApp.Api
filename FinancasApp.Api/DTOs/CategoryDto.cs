using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.DTOs;

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
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo é obrigatório")]
    [RegularExpression("^(Expense|Income)$", ErrorMessage = "Tipo deve ser 'Expense' ou 'Income'")]
    public string Type { get; set; } = "Expense";

    public string Icon { get; set; } = "other.png";
}