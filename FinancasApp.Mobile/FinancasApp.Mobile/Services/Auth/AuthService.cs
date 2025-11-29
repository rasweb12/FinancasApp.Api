// Services/AuthService.cs

// Services/AuthService.cs
using FinancasApp.Mobile.Models.DTOs;

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

            // CORRETO: LoginResponse já é o objeto retornado
            if (!string.IsNullOrEmpty(loginResponse?.Token))
            {
                Token = loginResponse.Token;
                await SecureStorage.SetAsync(Key, Token);
                return true;
            }
        }
        catch (Exception ex)
        {
        }
        return false;
    }

    public async Task SaveTokenAsync(string token)
    {
        Token = token;
        await SecureStorage.SetAsync(Key, token);
    }

    public async Task LoadTokenAsync()
    {
        Token = await SecureStorage.GetAsync(Key) ?? string.Empty;
    }

    public async Task LogoutAsync()
    {
        Token = string.Empty;
        SecureStorage.Remove(Key);
    }
}