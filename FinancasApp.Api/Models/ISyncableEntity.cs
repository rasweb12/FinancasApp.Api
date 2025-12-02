// Models/ISyncableEntity.cs
namespace FinancasApp.Api.Models;

public interface ISyncableEntity
{
    Guid Id { get; set; }
    bool IsNew { get; set; }
    bool IsDirty { get; set; }
    bool IsDeleted { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}