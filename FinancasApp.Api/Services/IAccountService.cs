using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;

namespace FinancasApp.Api.Services
{
    public interface IAccountService
    {
        Task SyncAsync(List<AccountDto> accounts, Guid userId);
        Task<List<Account>> GetAllAsync(Guid userId);
    }
}
