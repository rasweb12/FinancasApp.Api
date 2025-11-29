using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Models.DTOs;
using System;

namespace FinancasApp.Mobile.Mappers
{
    public static class InvoiceMapper
    {
        // --------------------------------------------------------------
        // DTO → LOCAL
        // --------------------------------------------------------------
        public static InvoiceLocal ToLocal(this InvoiceDto dto)
        {
            return new InvoiceLocal
            {
                Id = dto.Id,
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

        // --------------------------------------------------------------
        // LOCAL → DTO
        // --------------------------------------------------------------
        public static InvoiceDto ToDto(this InvoiceLocal local)
        {
            return new InvoiceDto
            {
                Id = local.Id,
                CreditCardId = local.CreditCardId,
                ClosingDate = local.ClosingDate,
                DueDate = local.DueDate,
                Total = local.Total,
                PaidAmount = local.PaidAmount,
                Month = local.Month,
                Year = local.Year,
                IsPaid = local.IsPaid,
                IsNew = local.IsNew,
                IsDirty = local.IsDirty,
                IsDeleted = local.IsDeleted,
                CreatedAt = local.CreatedAt,
                UpdatedAt = local.UpdatedAt
            };
        }

        // --------------------------------------------------------------
        // Atualiza local com valores do servidor
        // --------------------------------------------------------------
        public static void UpdateFromDto(this InvoiceLocal local, InvoiceDto dto)
        {
            local.CreditCardId = dto.CreditCardId;
            local.ClosingDate = dto.ClosingDate;
            local.DueDate = dto.DueDate;
            local.Total = dto.Total;
            local.PaidAmount = dto.PaidAmount;
            local.Month = dto.Month;
            local.Year = dto.Year;
            local.IsPaid = dto.IsPaid;
            local.IsNew = dto.IsNew;
            local.IsDirty = dto.IsDirty;
            local.IsDeleted = dto.IsDeleted;
            local.UpdatedAt = dto.UpdatedAt;
        }

        // --------------------------------------------------------------
        // Opcional: aplica alterações locais antes de enviar
        // --------------------------------------------------------------
        public static void ApplyLocalChanges(this InvoiceLocal local)
        {
            local.IsDirty = true;
            local.UpdatedAt = DateTime.UtcNow;
        }
    }
}
