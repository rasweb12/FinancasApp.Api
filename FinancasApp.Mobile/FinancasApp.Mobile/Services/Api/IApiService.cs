using FinancasApp.Mobile.Models.DTOs;
using Refit;

namespace FinancasApp.Mobile.Services.Api;

public interface IApiService
{
    // AUTH
    [Post("/auth/login")]
    Task<LoginResponse> LoginAsync([Body] LoginRequest request);

    [Post("/auth/register")]
    Task<RegisterResponse> RegisterAsync([Body] RegisterRequest request);

    // SYNC - DOWNLOAD
    [Get("/sync/accounts")]
    Task<List<AccountDto>> GetAccountsForSyncAsync();

    [Get("/sync/cards")]
    Task<List<CreditCardDto>> GetCardsForSyncAsync();

    [Get("/sync/transactions")]
    Task<List<TransactionDto>> GetTransactionsForSyncAsync();

    [Get("/sync/invoices")]
    Task<List<InvoiceDto>> GetInvoicesForSyncAsync();

    // SYNC - UPLOAD + DOWNLOAD (POST completo)
    [Post("/sync")]
    Task<SyncResponseDto> SyncAllAsync([Body] SyncRequestDto request);
}