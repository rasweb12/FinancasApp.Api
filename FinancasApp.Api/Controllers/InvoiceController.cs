using FinancasApp.Api.DTOs;
using FinancasApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinancasApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // --------------------------------------------------------------
        // GET ALL - api/invoices/all
        // --------------------------------------------------------------
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _invoiceService.GetAllAsync(userId);
            return Ok(result);
        }

        // --------------------------------------------------------------
        // GET BY ID - api/invoices/{id}
        // --------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _invoiceService.GetByIdAsync(id, userId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // --------------------------------------------------------------
        // UPSERT - api/invoices/upsert
        // --------------------------------------------------------------
        [HttpPost("upsert")]
        public async Task<IActionResult> Upsert([FromBody] InvoiceDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var ok = await _invoiceService.UpsertAsync(dto, userId);
            if (!ok)
                return BadRequest("Erro ao salvar invoice.");

            return Ok();
        }

        // --------------------------------------------------------------
        // DELETE - api/invoices/{id}
        // --------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var ok = await _invoiceService.DeleteAsync(id, userId);
            if (!ok)
                return BadRequest("Erro ao deletar invoice.");

            return Ok();
        }

        // --------------------------------------------------------------
        // SYNC - api/invoices/sync
        // --------------------------------------------------------------
        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromBody] List<InvoiceDto> incoming)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            await _invoiceService.SyncAsync(incoming, userId);

            return Ok();
        }
    }
}
