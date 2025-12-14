using System.Net.Http;
using CommunityToolkit.Mvvm.Messaging;
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
            var response = await _api.LoginAsync(new LoginRequest
            {
                Email = email.Trim(),
                Password = password
            });

            if (!string.IsNullOrWhiteSpace(response?.Token))
            {
                await SaveTokenAsync(response.Token);

                _logger.LogInformation("Login realizado com sucesso: {Email}", email);

                // 🔔 Notifica Shell / App
                WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());

                return LoginResult.Ok(response.Token);
            }

            return LoginResult.Error("E-mail ou senha inválidos");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de rede no login");
            return LoginResult.Error("Sem conexão com a internet");
        }
        catch (TaskCanceledException)
        {
            _logger.LogError("Timeout no login");
            return LoginResult.Error("Servidor demorou para responder");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado no login");
            return LoginResult.Error("Erro interno. Tente novamente.");
        }
    }

    public async Task SaveTokenAsync(string token)
    {
        Token = token;
        await SecureStorage.Default.SetAsync("jwt_token", token);
    }

    public async Task LoadTokenAsync()
    {
        Token = await SecureStorage.Default.GetAsync("jwt_token");
    }

    public Task<string?> GetTokenAsync()
        => Task.FromResult(Token);

    public async Task LogoutAsync()
    {
        SecureStorage.Default.Remove("jwt_token");
        Token = null;

        _logger.LogInformation("Logout efetuado");

        // 🔔 Notifica Shell / App
        WeakReferenceMessenger.Default.Send(new LogoutMessage());

        await Task.CompletedTask;
    }
}
