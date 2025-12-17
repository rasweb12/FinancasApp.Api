using AutoMapper;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancasApp.Api.Controllers;

[ApiController]
[Route("sync")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ITransactionService _transactionService;
    private readonly ICreditCardService _creditCardService;
    private readonly IInvoiceService _invoiceService;
    private readonly ICategoryService _categoryService; // ◄ ADICIONADO
    private readonly IMapper _mapper;
    private readonly ILogger<SyncController> _logger;

    public SyncController(
        IAccountService accountService,
        ITransactionService transactionService,
        ICreditCardService creditCardService,
        IInvoiceService invoiceService,
        ICategoryService categoryService, // ◄ INJETADO
        IMapper mapper,
        ILogger<SyncController> logger)
    {
        _accountService = accountService;
        _transactionService = transactionService;
        _creditCardService = creditCardService;
        _invoiceService = invoiceService;
        _categoryService = categoryService;
        _mapper = mapper;
        _logger = logger;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Sync([FromBody] SyncRequestDto request)
    {
        var userId = GetUserId();
        _logger.LogInformation("🔄 Sync iniciado para usuário {UserId}", userId);

        try
        {
            await Task.WhenAll(
                _accountService.SyncAsync(request.Accounts ?? new(), userId),
                _creditCardService.SyncAsync(request.CreditCards ?? new(), userId),
                _invoiceService.SyncAsync(request.Invoices ?? new(), userId),
                _transactionService.SyncAsync(request.Transactions ?? new(), userId),
                _categoryService.SyncAsync(request.Categories ?? new(), userId) // ◄ CATEGORIAS
            );

            var response = new SyncResponseDto
            {
                Accounts = _mapper.Map<List<AccountDto>>(await _accountService.GetAllAsync(userId)),
                CreditCards = _mapper.Map<List<CreditCardDto>>(await _creditCardService.GetAllAsync(userId)),
                Invoices = _mapper.Map<List<InvoiceDto>>(await _invoiceService.GetAllAsync(userId)),
                Transactions = _mapper.Map<List<TransactionDto>>(await _transactionService.GetAllAsync(userId)),
                Categories = _mapper.Map<List<CategoryDto>>(await _categoryService.GetAllAsync(userId)) // ◄ CATEGORIAS NO RESPONSE
            };

            _logger.LogInformation("✅ Sync concluído");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro no sync");
            return StatusCode(500, "Falha na sincronização");
        }
    }

    // GETs individuais
    [HttpGet("accounts")] public async Task<IActionResult> GetAccounts() => Ok(await _accountService.GetAllAsync(GetUserId()));
    [HttpGet("cards")] public async Task<IActionResult> GetCards() => Ok(await _creditCardService.GetAllAsync(GetUserId()));
    [HttpGet("transactions")] public async Task<IActionResult> GetTransactions() => Ok(await _transactionService.GetAllAsync(GetUserId()));
    [HttpGet("invoices")] public async Task<IActionResult> GetInvoices() => Ok(await _invoiceService.GetAllAsync(GetUserId()));
    [HttpGet("categories")] public async Task<IActionResult> GetCategories() => Ok(await _categoryService.GetAllAsync(GetUserId())); // ◄ NOVO
}