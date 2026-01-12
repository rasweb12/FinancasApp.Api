using SQLite;
using System;

namespace FinancasApp.Mobile.Models.Local;

public enum CategoryType
{
    Expense,
    Income
}

[Table("Categories")]
public class CategoryLocal : BaseEntity
{
    [PrimaryKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? UserId { get; set; }

    [Indexed]
    public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; } = CategoryType.Expense;

    public string Icon { get; set; } = "other.png";

    public bool IsSystem { get; set; } = false;

    public int TransactionCount { get; set; } = 0;
}