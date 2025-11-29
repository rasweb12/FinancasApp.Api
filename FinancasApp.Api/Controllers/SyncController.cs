using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using FinancasApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancasApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ITransactionService _transactionService;
    private readonly ICreditCardService _creditCardService;
    private readonly IInvoiceService _invoiceService;

    public SyncController(
        IAccountService accountService,
        ITransactionService transactionService,
        ICreditCardService creditCardService,
        IInvoiceService invoiceService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
        _creditCardService = creditCardService;
        _invoiceService = invoiceService;
    }

    private Guid GetUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    // =============================================================
    // ========================   SYNC   ============================
    // =============================================================
    [HttpPost]
    public async Task<IActionResult> Sync([FromBody] SyncRequestDto request)
    {
        var userId = GetUserId();

        // ---- ACCOUNTS ----------------------------------------------------------
        await _accountService.SyncAsync(request.Accounts, userId);

        // ---- CREDIT CARDS ------------------------------------------------------
        await _creditCardService.SyncAsync(request.CreditCards, userId);

        // ---- INVOICES ----------------------------------------------------------
        await _invoiceService.SyncAsync(request.Invoices, userId);

        // ---- TRANSACTIONS ------------------------------------------------------
        await _transactionService.SyncAsync(request.Transactions, userId);

        // ------------------------------------------------------------
        // RETORNAR TUDO ATUALIZADO PARA O CELULAR
        // ------------------------------------------------------------

        var accounts = await _accountService.GetAllAsync(userId);
        var creditCards = await _creditCardService.GetAllAsync(userId);
        var invoices = await _invoiceService.GetAllAsync(userId);
        var transactions = await _transactionService.GetAllAsync(userId);

        return Ok(new SyncResponseDto
        {
            Accounts = accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name,
                Balance = a.Balance,
                AccountType = a.AccountType,
                IsNew = false,
                IsDirty = false,
                IsDeleted = false
            }).ToList(),

            CreditCards = creditCards.Select(c => new CreditCardDto
            {
                Id = c.Id,
                Name = c.Name,
                CreditLimit = c.CreditLimit,
                ClosingDay = c.ClosingDay,
                DueDay = c.DueDay,
                IsNew = false,
                IsDirty = false,
                IsDeleted = false
            }).ToList(),

            Invoices = invoices.Select(i => new InvoiceDto
            {
                Id = i.Id,
                CreditCardId = i.CreditCardId,
                Month = i.Month,
                Year = i.Year,
                Total = i.Total,
                IsNew = false,
                IsDirty = false,
                IsDeleted = false
            }).ToList(),

            Transactions = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Description = t.Description ?? "",
                Amount = t.Amount,
                Date = t.Date,
                Type = t.Type.ToString(),
                SubType = t.SubType,
                Tags = t.Tags,
                InstallmentNumber = t.InstallmentNumber,
                InstallmentTotal = t.InstallmentTotal,
                IsRecurring = t.IsRecurring,
                TransactionGroupId = t.TransactionGroupId,
                IsNew = false,
                IsDirty = false,
                IsDeleted = false
            }).ToList()
        });
    }
}
