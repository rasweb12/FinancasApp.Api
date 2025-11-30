// Services/Api/ITransactionApiService.cs
using FinancasApp.Mobile.Models.DTOs;

namespace FinancasApp.Mobile.Services.Api;

public interface ITransactionApiService
{
    Task<List<TransactionDto>> GetAllAsync();
    Task UpsertAsync(TransactionDto dto);
    Task DeleteAsync(Guid id);
}