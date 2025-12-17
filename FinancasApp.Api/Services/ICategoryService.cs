using FinancasApp.Api.DTOs;

namespace FinancasApp.Api.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetByUserAsync(Guid userId);
    Task<List<CategoryDto>> GetForSyncAsync(Guid userId);
    Task<List<CategoryDto>> GetAllAsync(Guid userId); // ◄ NOVO: Para SyncController

    Task<CategoryDto> CreateAsync(CategoryDto dto, Guid userId);
    Task UpdateAsync(CategoryDto dto);
    Task SoftDeleteAsync(Guid id);
    Task SyncAsync(List<CategoryDto> dtos, Guid userId);
}