// Controllers/AccountsController.cs
using AutoMapper;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancasApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper; // ← ADICIONADO

    public AccountsController(IAccountService accountService, IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper; // ← INJETADO
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAll()
    {
        var userId = GetUserId();
        var accounts = await _accountService.GetAllAsync(userId);
        var dtos = _mapper.Map<List<AccountDto>>(accounts); // ← Mapeia Entity → DTO
        return Ok(dtos);
    }

    [HttpPost("sync")]
    public async Task<ActionResult<SyncResponseDto>> Sync([FromBody] List<AccountDto> accounts)
    {
        var userId = GetUserId();
        await _accountService.SyncAsync(accounts, userId);

        var updatedEntities = await _accountService.GetAllAsync(userId);
        var updatedDtos = _mapper.Map<List<AccountDto>>(updatedEntities); // ← Mapeia aqui

        return Ok(new SyncResponseDto
        {
            Accounts = updatedDtos // ← Agora é List<AccountDto>
        });
    }
}