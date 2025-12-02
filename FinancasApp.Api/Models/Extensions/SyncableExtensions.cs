// Models/Extensions/SyncableExtensions.cs
namespace FinancasApp.Api.Models.Extensions;

public static class SyncableExtensions
{
    public static void MarkAsSynced(this ISyncableEntity entity)
    {
        entity.IsNew = false;
        entity.IsDirty = false;
        entity.IsDeleted = false;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static void MarkAsDirty(this ISyncableEntity entity)
    {
        entity.IsDirty = true;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static void MarkAsDeleted(this ISyncableEntity entity)
    {
        entity.IsDeleted = true;
        entity.IsDirty = true;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
