using FinancasApp.Mobile.Models.Local;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Mappers;
using FinancasApp.Mobile.Services.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancasApp.Mobile.Data;

namespace FinancasApp.Mobile.Services.Sync
{
    public class InvoiceSyncService
    {
        private readonly InvoiceLocalRepository _localRepo;
        private readonly InvoiceApiService _apiService;

        public InvoiceSyncService(
            InvoiceLocalRepository localRepo,
            InvoiceApiService apiService)
        {
            _localRepo = localRepo;
            _apiService = apiService;
        }

        // ---------------------------------------------------------------------
        // 1) Enviar INVOICES locais para API
        // ---------------------------------------------------------------------
        public async Task PushAsync()
        {
            var dirtyInvoices = await _localRepo.GetDirtyAsync();
            if (!dirtyInvoices.Any()) return;

            foreach (var local in dirtyInvoices)
            {
                var dto = local.ToDto();

                // Se está deletado → usa endpoint DELETE /invoices/{id}
                if (local.IsDeleted)
                {
                    await _apiService.DeleteAsync(local.Id);
                }
                else
                {
                    await _apiService.UpsertAsync(dto);
                }

                // Após envio bem sucedido
                local.IsDirty = false;
                local.IsNew = false;
                await _localRepo.UpdateAsync(local);
            }
        }

        // ---------------------------------------------------------------------
        // 2) Baixar INVOICES da API
        // ---------------------------------------------------------------------
        public async Task PullAsync()
        {
            var serverInvoices = await _apiService.GetAllAsync();
            foreach (var dto in serverInvoices)
            {
                var local = await _localRepo.GetByIdAsync(dto.Id);

                if (local == null)
                {
                    // Novo registro vindo da API
                    var localNew = dto.ToLocal();
                    await _localRepo.InsertAsync(localNew);
                    continue;
                }

                // Conflito: UpdatedAt define quem vence
                if (dto.UpdatedAt > local.UpdatedAt)
                {
                    // Servidor vence
                    local.UpdateFromDto(dto);
                    await _localRepo.UpdateAsync(local);
                }
                else
                {
                    // Local vence → marca dirty
                    local.IsDirty = true;
                    await _localRepo.UpdateAsync(local);
                }
            }
        }

        // ---------------------------------------------------------------------
        // 3) Sync completo (chamado pelo App)
        // ---------------------------------------------------------------------
        public async Task SyncAsync()
        {
            await PushAsync();  // envia alterações locais primeiro
            await PullAsync();  // depois baixa alterações do servidor
        }
    }
}
