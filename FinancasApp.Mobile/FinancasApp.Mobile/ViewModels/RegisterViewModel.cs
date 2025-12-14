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
                Email = Email.Trim(),
                Password = Password
            };

            // Realizando o registro
            var response = await _api.RegisterAsync(request);

            // Verifica se a resposta foi bem-sucedida
            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                ErrorMessage = "Erro ao criar conta. Tente novamente.";
                HasError = true;
                return;
            }

            // Salva o token de autenticação
            await _auth.SaveTokenAsync(response.Content.Token);

            // Navega para a tela inicial
            await Shell.Current.GoToAsync("//home");
        }
        catch (ApiException apiEx)
        {
            HasError = true;
            ErrorMessage = "Erro ao comunicar com o servidor.";

            _logger.LogError(apiEx,
                "Erro HTTP no registro | StatusCode: {StatusCode} | Content: {Content}",
                apiEx.StatusCode,
                apiEx.Content);
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Erro inesperado. Tente novamente.";

            _logger.LogError(ex, "Erro inesperado no registro | Message: {Message} | Inner: {Inner}",
                ex.Message, ex.InnerException?.Message);
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
