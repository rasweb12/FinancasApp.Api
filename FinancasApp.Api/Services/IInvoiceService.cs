using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;

namespace FinancasApp.Api.Services;

public interface IInvoiceService
{
    Task<Invoice?> GetByIdAsync(Guid id, Guid userId);
    Task<Invoice> AddAsync(Invoice invoice);
    Task<Invoice> UpdateAsync(Invoice invoice);
    Task<bool> DeleteAsync(Guid id, Guid userId);

    // Upsert individual
    Task<bool> UpsertAsync(InvoiceDto dto, Guid userId);

    // Sync em lote
    Task SyncAsync(List<InvoiceDto> invoices, Guid userId);
    Task<List<Invoice>> GetAllAsync(Guid userId);
}
