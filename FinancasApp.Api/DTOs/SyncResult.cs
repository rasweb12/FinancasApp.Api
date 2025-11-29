namespace FinancasApp.Api.DTOs;

public class SyncResult
{
    public Guid Id { get; set; }
    public string Entity { get; set; } = "";
    public string Status { get; set; } = ""; // created | updated | deleted
}

