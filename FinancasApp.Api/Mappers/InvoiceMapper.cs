using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;

namespace FinancasApp.Api.Mappers
{
    public static class InvoiceMapper
    {
        public static void UpdateFromDto(this Invoice model, InvoiceDto dto)
        {
            model.CreditCardId = dto.CreditCardId;
            model.ClosingDate = dto.ClosingDate;
            model.DueDate = dto.DueDate;
            model.Total = dto.Total;
            model.PaidAmount = dto.PaidAmount;
            model.Month = dto.Month;
            model.Year = dto.Year;

            model.IsPaid = dto.IsPaid;
            model.IsNew = dto.IsNew;
            model.IsDirty = dto.IsDirty;
            model.IsDeleted = dto.IsDeleted;

            model.UpdatedAt = DateTime.UtcNow;
        }

        public static Invoice ToModel(this InvoiceDto dto, string userId)
        {
            return new Invoice
            {
                Id = dto.Id,
                UserId = dto.UserId,

                CreditCardId = dto.CreditCardId,
                ClosingDate = dto.ClosingDate,
                DueDate = dto.DueDate,
                Total = dto.Total,
                PaidAmount = dto.PaidAmount,
                Month = dto.Month,
                Year = dto.Year,

                IsPaid = dto.IsPaid,
                IsNew = dto.IsNew,
                IsDirty = dto.IsDirty,
                IsDeleted = dto.IsDeleted,

                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }
    }
}
