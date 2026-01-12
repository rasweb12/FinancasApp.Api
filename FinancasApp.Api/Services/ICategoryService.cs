using FinancasApp.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinancasApp.Api.Services;

public interface ICategoryService
{
    /// <summary>
    /// Retorna categorias do usuário + system (não deletadas)
    /// </summary>
    Task<List<CategoryDto>> GetByUserAsync(Guid userId);

    /// <summary>
    /// Alias para GetByUserAsync (compatibilidade com controladores antigos)
    /// </summary>
    Task<List<CategoryDto>> GetAllAsync(Guid userId);

    /// <summary>
    /// Retorna categorias para sincronização (mesmo filtro que GetByUserAsync)
    /// </summary>
    Task<List<CategoryDto>> GetForSyncAsync(Guid userId);

    /// <summary>
    /// Cria nova categoria a partir de request simples (recomendado)
    /// </summary>
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request, Guid userId);

    /// <summary>
    /// Atualiza categoria existente (via DTO completo)
    /// </summary>
    Task UpdateAsync(CategoryDto dto);

    /// <summary>
    /// Soft delete (marca IsDeleted = true)
    /// </summary>
    Task SoftDeleteAsync(Guid id);

    /// <summary>
    /// Sincroniza lista de categorias do mobile com o servidor
    /// </summary>
    Task SyncAsync(List<CategoryDto> dtos, Guid userId);
}