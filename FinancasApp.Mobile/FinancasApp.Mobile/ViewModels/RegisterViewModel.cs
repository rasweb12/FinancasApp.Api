using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.Auth;
using Microsoft.Extensions.Logging;
using Refit;
using System.Net;

namespace FinancasApp.Mobile.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IApiService _api;
    private readonly IAuthService _auth;
    private readonly ILogger<RegisterViewModel> _logger;

    [ObservableProperty] private string fullName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool hasError;

    public RegisterViewModel(
        IApiService api,
        IAuthService auth,
        ILogger<RegisterViewModel> logger)
    {
        _api = api;
        _auth = auth;
        _logger = logger;
    }

    private bool CanRegister() =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(FullName) &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password) &&
        Password == ConfirmPassword;

    [RelayCommand(CanExecute = nameof(CanRegister))]
    private async Task RegisterAsync()
    {
        try
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var request = new RegisterRequest
            {
                FullName = FullName.Trim(),
                Email = Email.Trim().ToLowerInvariant(),  // ← Padroniza e-mail
                Password = Password
            };

            var response = await _api.RegisterAsync(request);

            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                ErrorMessage = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => "Dados inválidos (verifique os campos).",
                    HttpStatusCode.Conflict => "E-mail já cadastrado.",
                    HttpStatusCode.NotFound => "Endpoint de registro não encontrado (verifique IApiService).",
                    HttpStatusCode.InternalServerError => "Erro interno no servidor.",
                    _ => $"Falha na API: {response.StatusCode}"
                };
                HasError = true;
                return;
            }

            await _auth.SaveTokenAsync(response.Content.Token);
            await Shell.Current.GoToAsync("//home");  // Agora funciona!
        }
        catch (ApiException apiEx)
        {
            HasError = true;
            ErrorMessage = $"Erro de API: {apiEx.StatusCode} - {apiEx.Content}";
            _logger.LogError(apiEx, "ApiException no register: {Content}", apiEx.Content);
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Erro inesperado: {ex.Message}";
            _logger.LogError(ex, "Exceção no register");
        }
        finally
        {
            IsBusy = false;
            RegisterCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private Task BackToLogin() =>
        Shell.Current.GoToAsync("//login");

    partial void OnFullNameChanged(string _) => RegisterCommand.NotifyCanExecuteChanged();
    partial void OnEmailChanged(string _) => RegisterCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string _) => RegisterCommand.NotifyCanExecuteChanged();
    partial void OnConfirmPasswordChanged(string _) => RegisterCommand.NotifyCanExecuteChanged();
}
