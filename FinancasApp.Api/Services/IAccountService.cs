// Services/IAccountService.cs
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;

namespace FinancasApp.Api.Services;

// IAccountService.cs — FIQUE SÓ COM ISSO:
public interface IAccountService
{
    Task SyncAsync(List<AccountDto> accounts, Guid userId);
    Task<List<Account>> GetAllAsync(Guid userId);
}
