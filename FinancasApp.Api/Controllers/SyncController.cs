// Controllers/SyncController.cs
using FinancasApp.Api.DTOs;
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

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Sync([FromBody] SyncRequestDto request)
    {
        var userId = GetUserId();

        // UPLOAD: sincroniza dados do celular → servidor
        await _accountService.SyncAsync(request.Accounts, userId);
        await _creditCardService.SyncAsync(request.CreditCards, userId);
        await _invoiceService.SyncAsync(request.Invoices, userId);
        await _transactionService.SyncAsync(request.Transactions, userId);

        // DOWNLOAD: retorna tudo atualizado pro celular
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
                AccountType = a.AccountType,
                Currency = a.Currency,
                Balance = a.Balance,
                InitialBalance = a.InitialBalance,
                IsDeleted = a.IsDeleted,
                UpdatedAt = a.UpdatedAt,
                CreatedAt = a.CreatedAt
            }).ToList(),

            CreditCards = creditCards.Select(c => new CreditCardDto
            {
                Id = c.Id,
                Name = c.Name,
                Last4Digits = c.Last4Digits,
                CreditLimit = c.CreditLimit,
                DueDay = c.DueDay,
                ClosingDay = c.ClosingDay,
                CurrentInvoiceId = c.CurrentInvoiceId,
                IsDeleted = c.IsDeleted,
                UpdatedAt = c.UpdatedAt,
                CreatedAt = c.CreatedAt
            }).ToList(),

            Invoices = invoices.Select(i => new InvoiceDto
            {
                Id = i.Id,
                CreditCardId = i.CreditCardId,
                Month = i.Month,
                Year = i.Year,
                Total = i.Total,
                PaidAmount = i.PaidAmount,
                IsPaid = i.IsPaid,
                IsDeleted = i.IsDeleted,
                UpdatedAt = i.UpdatedAt,
                CreatedAt = i.CreatedAt
            }).ToList(),

            Transactions = transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Description = t.Description ?? "",
                Amount = t.Amount,
                Date = t.Date,
                CategoryId = t.Category?.Id,           // ← CORRETO: manda o ID da categoria
                Category = t.Category?.Name ?? "",  // ← manda o nome pra exibir no mobile
                Type = t.Type.ToString(),
                SubType = t.SubType,
                Tags = t.Tags,
                InstallmentNumber = t.InstallmentNumber,
                InstallmentTotal = t.InstallmentTotal,
                TransactionGroupId = t.TransactionGroupId,
                IsRecurring = t.IsRecurring,
                IsDeleted = t.IsDeleted,
                UpdatedAt = t.UpdatedAt,
                CreatedAt = t.CreatedAt
            }).ToList()
        });
    }
}