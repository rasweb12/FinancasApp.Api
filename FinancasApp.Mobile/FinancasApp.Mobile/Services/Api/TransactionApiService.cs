// Services/Api/TransactionApiService.cs
using FinancasApp.Mobile.Models.DTOs;
using Refit;

namespace FinancasApp.Mobile.Services.Api;

public interface ITransactionApiClient
{
    [Get("/transactions")]
    Task<List<TransactionDto>> GetAllAsync();

    [Post("/transactions")]
    Task UpsertAsync([Body] TransactionDto dto);

    [Delete("/transactions/{id}")]
    Task DeleteAsync(Guid id);
}

public class TransactionApiService : ITransactionApiService
{
    private readonly ITransactionApiClient _client;

    public TransactionApiService(HttpClient httpClient)
    {
        _client = RestService.For<ITransactionApiClient>(httpClient);
    }

    public Task<List<TransactionDto>> GetAllAsync() => _client.GetAllAsync();
    public Task UpsertAsync(TransactionDto dto) => _client.UpsertAsync(dto);
    public Task DeleteAsync(Guid id) => _client.DeleteAsync(id);
}