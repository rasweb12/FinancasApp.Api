using System.Net.Http;
using System.Text.Json;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Services.Api;
using Microsoft.Extensions.Logging;

namespace FinancasApp.Mobile.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IApiService _api;
    private readonly ILogger<AuthService> _logger;

    public string? Token { get; private set; }

    public AuthService(IApiService api, ILogger<AuthService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest
            {
                Email = email.Trim(),
                Password = password
            };

            var response = await _api.LoginAsync(request);

            if (response is { Token: not null })
            {
                Token = response.Token;
                await SaveTokenAsync(Token);

                _logger.LogInformation("Login bem-sucedido para {Email}", email);

                return new LoginResult
                {
                    Success = true,
                    Token = Token
                };
            }

            return new LoginResult
            {
                Success = false,
                Message = "E-mail ou senha inválidos"
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede ao fazer login");
            return new LoginResult
            {
                Success = false,
                Message = "Sem conexão com a internet"
            };
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout ao fazer login");
            return new LoginResult
            {
                Success = false,
                Message = "Servidor demorou para responder"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao fazer login");
            return new LoginResult
            {
                Success = false,
                Message = "Erro interno. Tente novamente."
            };
        }
    }

    public async Task SaveTokenAsync(string token)
    {
        await SecureStorage.Default.SetAsync("jwt_token", token);
        Token = token;
    }

    public async Task LoadTokenAsync()
    {
        Token = await SecureStorage.Default.GetAsync("jwt_token");

        if (!string.IsNullOrEmpty(Token))
        {
            _logger.LogInformation("Token carregado do SecureStorage");
        }
    }

    public async Task LogoutAsync()
    {
        SecureStorage.Default.Remove("jwt_token");
        Token = null;
        _logger.LogInformation("Logout realizado com sucesso");
    }
}