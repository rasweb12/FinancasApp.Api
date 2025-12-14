using FinancasApp.Mobile.Models.DTOs;
using Refit;

public interface IApiService
{
    // AUTH
    [Post("/auth/login")]
    Task<ApiResponse<LoginResponse>> LoginAsync([Body] LoginRequest request);

    [Post("/auth/register")]
    Task<ApiResponse<RegisterResponse>> RegisterAsync([Body] RegisterRequest request);

    // SYNC
    [Get("/sync/accounts")]
    Task<ApiResponse<List<AccountDto>>> GetAccountsForSyncAsync();

    [Get("/sync/cards")]
    Task<ApiResponse<List<CreditCardDto>>> GetCardsForSyncAsync();

    [Get("/sync/transactions")]
    Task<ApiResponse<List<TransactionDto>>> GetTransactionsForSyncAsync();

    [Get("/sync/invoices")]
    Task<ApiResponse<List<InvoiceDto>>> GetInvoicesForSyncAsync();

    [Post("/sync")]
    Task<ApiResponse<SyncResponseDto>> SyncAllAsync([Body] SyncRequestDto request);
}
