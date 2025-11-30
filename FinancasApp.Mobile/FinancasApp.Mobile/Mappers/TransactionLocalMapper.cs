using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Models.Local;

namespace FinancasApp.Mobile.Mappers
{
    public static class TransactionLocalMapper
    {
        // LOCAL → DTO (para upload)
        public static TransactionDto ToDto(TransactionLocal t)
        {
            return new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Description = t.Description,
                Date = t.Date,
                CategoryId = t.CategoryId,
                Tags = t.Tags,
                IsDeleted = t.IsDeleted,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            };
        }

        // DTO → LOCAL (para download)
        public static TransactionLocal ToLocal(TransactionDto d)
        {
            return new TransactionLocal
            {
                Id = d.Id,
                Amount = d.Amount,
                Description = d.Description,
                Date = d.Date,
                CategoryId = d.CategoryId,
                Tags = d.Tags ?? "",

                // CAMPOS SOMENTE LOCAIS
                // preservamos valores default para manter integridade
                Category = string.Empty,
                Type = string.Empty,
                SubType = null,
                InstallmentNumber = null,
                InstallmentTotal = null,
                TransactionGroupId = null,
                IsRecurring = false,

                // FLAGS DE SINCRONIZAÇÃO
                //IsNew = false,
                IsDirty = false,
                IsDeleted = d.IsDeleted,

                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            };
        }
    }
}
