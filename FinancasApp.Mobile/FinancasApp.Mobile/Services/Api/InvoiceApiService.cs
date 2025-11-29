using FinancasApp.Mobile.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FinancasApp.Mobile.Services.Api
{
    public class InvoiceApiService
    {
        private readonly HttpClient _http;

        public InvoiceApiService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        // ---------------------------------------------------------------------
        // GET: /api/invoices/all
        // ---------------------------------------------------------------------
        public async Task<List<InvoiceDto>> GetAllAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<List<InvoiceDto>>("api/invoices/all");
                return result ?? new List<InvoiceDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar invoices: {ex.Message}");
                return new List<InvoiceDto>();
            }
        }

        // ---------------------------------------------------------------------
        // POST/PUT: /api/invoices/upsert
        // ---------------------------------------------------------------------
        public async Task<bool> UpsertAsync(InvoiceDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/invoices/upsert", dto);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erro ao enviar invoice: {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro UpsertInvoice: {ex.Message}");
                return false;
            }
        }

        // ---------------------------------------------------------------------
        // DELETE: /api/invoices/{id}
        // ---------------------------------------------------------------------
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var response = await _http.DeleteAsync($"api/invoices/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Erro ao deletar invoice: {response.StatusCode}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro DeleteInvoice: {ex.Message}");
                return false;
            }
        }
    }
}
