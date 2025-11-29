using FinancasApp.Mobile.Models.DTOs;
using Refit;

public interface IApiService
{
    // ==============================
    // SYNC (para SyncService)
    // ==============================

    // Transactions
    [Get("/sync/transactions")]
    Task<List<TransactionDto>> GetTransactionsForSyncAsync();

    [Post("/sync/transactions")]
    Task<ApiResponse<object>> SyncTransactionsAsync([Body] List<TransactionDto> items);

    // Accounts
    [Get("/sync/accounts")]
    Task<List<AccountDto>> GetAccountsForSyncAsync();

    [Post("/sync/accounts")]
    Task<ApiResponse<object>> SyncAccountsAsync([Body] List<AccountDto> items);

    // Cards
    [Get("/sync/cards")]
    Task<List<CreditCardDto>> GetCardsForSyncAsync();

    [Post("/sync/cards")]
    Task<ApiResponse<object>> SyncCardsAsync([Body] List<CreditCardDto> items);

    // Invoices
    [Get("/sync/invoices")]
    Task<List<InvoiceDto>> GetInvoicesForSyncAsync();

    [Post("/sync/invoices")]
    Task<ApiResponse<object>> SyncInvoicesAsync([Body] List<InvoiceDto> items);


    // ==============================
    // BASIC REST ENDPOINTS
    // ==============================

    [Get("/transactions")]
    Task<List<TransactionDto>> GetTransactionsAsync();

    [Get("/accounts")]
    Task<List<AccountDto>> GetAccountsAsync();


    // ==============================
    // AUTH
    // ==============================

    [Post("/auth/login")]
    Task<LoginResponse> LoginAsync([Body] LoginRequest request);
}
