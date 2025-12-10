// Controllers/SyncController.cs — API
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
    private readonly ICreditCardService _creditCardService; // ← CORRIGIDO: era "creditService"
    private readonly IInvoiceService _invoiceService;

    public SyncController(
        IAccountService accountService,
        ITransactionService transactionService,
        ICreditCardService creditCardService, // ← CORRIGIDO
        IInvoiceService invoiceService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
        _creditCardService = creditCardService; // ← CORRIGIDO
        _invoiceService = invoiceService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Sync([FromBody] SyncRequestDto request)
    {
        var userId = GetUserId();

        await Task.WhenAll(
            _accountService.SyncAsync(request.Accounts, userId),
            _creditCardService.SyncAsync(request.CreditCards, userId),
            _invoiceService.SyncAsync(request.Invoices, userId),
            _transactionService.SyncAsync(request.Transactions, userId)
        );

        var accounts = await _accountService.GetAllAsync(userId);
        var creditCards = await _creditCardService.GetAllAsync(userId);
        var invoices = await _invoiceService.GetAllAsync(userId);
        var transactions = await _transactionService.GetAllAsync(userId);

        return Ok(new SyncResponseDto
        {
            Accounts = accounts.Select(a => new AccountDto { Id = a.Id, Name = a.Name, Balance = a.Balance, /* ... */ }).ToList(),
            CreditCards = creditCards.Select(c => new CreditCardDto { Id = c.Id, Name = c.Name, Last4Digits = c.Last4Digits, /* ... */ }).ToList(),
            Invoices = invoices.Select(i => new InvoiceDto { Id = i.Id, /* ... */ }).ToList(),
            Transactions = transactions.Select(t => new TransactionDto { Id = t.Id, Description = t.Description ?? "", Amount = t.Amount, Date = t.Date, /* ... */ }).ToList()
        });
    }

    // GETs individuais
    [HttpGet("accounts")] public async Task<IActionResult> GetAccounts() => Ok(await _accountService.GetAllAsync(GetUserId()));
    [HttpGet("cards")] public async Task<IActionResult> GetCards() => Ok(await _creditCardService.GetAllAsync(GetUserId()));
    [HttpGet("transactions")] public async Task<IActionResult> GetTransactions() => Ok(await _transactionService.GetAllAsync(GetUserId()));
    [HttpGet("invoices")] public async Task<IActionResult> GetInvoices() => Ok(await _invoiceService.GetAllAsync(GetUserId()));
}