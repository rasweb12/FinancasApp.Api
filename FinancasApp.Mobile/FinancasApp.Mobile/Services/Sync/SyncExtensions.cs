using FinancasApp.Mobile.Models.DTOs;

namespace FinancasApp.Mobile.Services.Sync;

public static class SyncExtensions
{
    public static bool HasAnyDirtyData(this SyncRequestDto request)
    {
        return request.Accounts.Any() ||
               request.CreditCards.Any() ||
               request.Invoices.Any() ||
               request.Transactions.Any() ||
               request.Categories.Any();
    }
}