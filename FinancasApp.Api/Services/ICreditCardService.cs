using FinancasApp.Api.DTOs;

namespace FinancasApp.Api.Services;

public interface ICreditCardService
{
    Task<List<CreditCardDto>> GetByUserAsync(Guid userId);     // Para endpoints normais
    Task<List<CreditCardDto>> GetForSyncAsync(Guid userId);    // Para sync (mesmo filtro)
    Task<List<CreditCardDto>> GetAllAsync(Guid userId);        // ◄ NOVO: Compatibilidade com SyncController

    Task<CreditCardDto> CreateAsync(CreditCardDto dto, Guid userId);
    Task UpdateAsync(CreditCardDto dto);
    Task SoftDeleteAsync(Guid id);
    Task SyncAsync(List<CreditCardDto> dtos, Guid userId);
}