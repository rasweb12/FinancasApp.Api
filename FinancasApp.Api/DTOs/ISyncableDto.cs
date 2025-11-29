namespace FinancasApp.Api.DTOs
{
    public interface ISyncableDto
    {
        Guid Id { get; set; }
        bool IsNew { get; set; }
        bool IsDirty { get; set; }
        bool IsDeleted { get; set; }
    }
}
