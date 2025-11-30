using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Services.Storage
{
    public interface ILocalStorageService
    {
        // GENERIC CRUD — válidos para QUALQUER entidade
        Task<T?> GetByIdAsync<T>(Guid id) where T : BaseEntity, new();
        Task<List<T>> GetAllAsync<T>() where T : BaseEntity, new();
        Task<int> SaveAsync<T>(T entity) where T : BaseEntity, new();
        Task<int> DeleteAsync<T>(Guid id) where T : BaseEntity, new();

        // ESPECÍFICOS APENAS ONDE NECESSÁRIO

        // Invoices — precisam listagem especial
        Task<List<InvoiceLocal>> GetPendingInvoicesAsync();
    }
}
