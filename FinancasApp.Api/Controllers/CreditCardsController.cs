using FinancasApp.Api.DTOs;
using FinancasApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancasApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CreditCardsController : ControllerBase
{
    private readonly ICreditCardService _service;
    private readonly ILogger<CreditCardsController> _logger;

    public CreditCardsController(ICreditCardService service, ILogger<CreditCardsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<CreditCardDto>>> GetCreditCards()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var cards = await _service.GetByUserAsync(userId);
        return Ok(cards);
    }

    [HttpGet("sync")]
    public async Task<ActionResult<List<CreditCardDto>>> GetForSync()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var cards = await _service.GetForSyncAsync(userId);
        return Ok(cards);
    }

    [HttpPost]
    public async Task<ActionResult<CreditCardDto>> CreateCreditCard(CreditCardDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var created = await _service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetCreditCard), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CreditCardDto>> GetCreditCard(Guid id)
    {
        // Implementar se necessário
        return Ok();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCreditCard(Guid id, CreditCardDto dto)
    {
        if (id != dto.Id) return BadRequest();
        await _service.UpdateAsync(dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCreditCard(Guid id)
    {
        await _service.SoftDeleteAsync(id);
        return NoContent();
    }
}