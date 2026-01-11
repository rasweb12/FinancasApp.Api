using System.Net;
using CommunityToolkit.Mvvm.Messaging;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Services.Api;
using Microsoft.Extensions.Logging;
using Refit;

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
        _ = LoadTokenAsync(); // Carrega token ao iniciar
    }

    public async Task<LoginResult> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _api.LoginAsync(new LoginRequest
            {
                Email = email.Trim().ToLowerInvariant(),
                Password = password
            });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha no login | Status: {Status}", response.StatusCode);
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => LoginResult.Error("E-mail ou senha inválidos"),
                    HttpStatusCode.BadRequest => LoginResult.Error("Dados inválidos"),
                    _ => LoginResult.Error("Erro no servidor")
                };
            }

            var token = response.Content?.Token;
            if (string.IsNullOrWhiteSpace(token))
                return LoginResult.Error("Token não retornado");

            await SaveTokenAsync(token);
            _logger.LogInformation("Login sucesso | Token salvo | Email: {Email}", email);

            WeakReferenceMessenger.Default.Send(new LoginSuccessMessage());

            return LoginResult.Ok(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no login");
            return LoginResult.Error("Falha na comunicação");
        }
    }

    public async Task SaveTokenAsync(string token)
    {
        Token = token;
        await SecureStorage.Default.SetAsync("jwt_token", token);
        _logger.LogInformation("Token salvo em SecureStorage | Length: {Length}", token.Length);
    }

    public async Task LoadTokenAsync()
    {
        Token = await SecureStorage.Default.GetAsync("jwt_token");
        _logger.LogInformation("Token carregado | {Status}", string.IsNullOrWhiteSpace(Token) ? "NULO" : "VÁLIDO");
    }

    public Task<string?> GetTokenAsync() => Task.FromResult(Token);

    public async Task LogoutAsync()
    {
        SecureStorage.Default.Remove("jwt_token");
        Token = null;
        _logger.LogInformation("Logout completo | Token removido");
        WeakReferenceMessenger.Default.Send(new LogoutMessage());
        await Task.CompletedTask;
    }
}
