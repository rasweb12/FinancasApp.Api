using FinancasApp.Mobile.Models.DTOs;
using Refit; // Para ApiException

namespace FinancasApp.Mobile.Services.Auth;

public class AuthService : IAuthService
{
    private const string Key = "jwt_token";
    private readonly IApiService _api;

    public string Token { get; private set; } = string.Empty;

    public AuthService(IApiService api)
    {
        _api = api;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var loginResponse = await _api.LoginAsync(new LoginRequest
            {
                Email = email,
                Password = password
            });

            Console.WriteLine($"[AUTH] Resposta recebida: Token={(string.IsNullOrEmpty(loginResponse?.Token) ? "NULO" : "OK")}, UserId={loginResponse?.UserId}");

            if (!string.IsNullOrEmpty(loginResponse?.Token))
            {
                Token = loginResponse.Token;
                await SecureStorage.Default.SetAsync(Key, Token);
                Console.WriteLine("[AUTH] Login bem-sucedido! Token salvo.");
                return true;
            }
            else
            {
                Console.WriteLine("[AUTH] Token veio vazio ou nulo");
                return false;
            }
        }
        catch (ApiException apiEx)
        {
            Console.WriteLine($"[AUTH] Erro API: {apiEx.StatusCode} - {apiEx.Content}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH] Exceção: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }

    public async Task SaveTokenAsync(string token)
    {
        Token = token;
        await SecureStorage.Default.SetAsync(Key, token);
    }

    public async Task LoadTokenAsync()
    {
        Token = await SecureStorage.Default.GetAsync(Key) ?? string.Empty;
    }

    public async Task LogoutAsync()
    {
        Token = string.Empty;
        SecureStorage.Default.Remove(Key);
    }
}