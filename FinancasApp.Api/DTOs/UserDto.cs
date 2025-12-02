// FinancasApp.Api/DTOs/UserDto.cs
using FinancasApp.Api.Models;

namespace FinancasApp.Api.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }

    // Campos de sync (obrigatórios pro mobile)
    public bool IsNew { get; set; } = true;
    public bool IsDirty { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
