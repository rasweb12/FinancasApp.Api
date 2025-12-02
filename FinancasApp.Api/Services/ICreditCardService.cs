using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;

namespace FinancasApp.Api.Services;

public interface ICreditCardService
{
    Task<CreditCard?> GetByIdAsync(Guid id, Guid userId);
    Task<CreditCard> AddAsync(CreditCard card);
    Task<CreditCard> UpdateAsync(CreditCard card);
    Task<bool> DeleteAsync(Guid id, Guid userId);

    // Sync
    Task SyncAsync(List<CreditCardDto> cards, Guid userId);
    Task<List<CreditCard>> GetAllAsync(Guid userId);
}
