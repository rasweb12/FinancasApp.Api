using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancasApp.Mobile.Models.DTOs;
using FinancasApp.Mobile.Services.Api;
using FinancasApp.Mobile.Services.Auth;
using Microsoft.Extensions.Logging;
using Refit;
using System.Net;

namespace FinancasApp.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IApiService _api;
    private readonly IAuthService _auth;
    private readonly ILogger<LoginViewModel> _logger;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool hasError;

    public LoginViewModel(
        IApiService api,
        IAuthService auth,
        ILogger<LoginViewModel> logger)
    {
        _api = api;
        _auth = auth;
        _logger = logger;
    }

    private bool CanLogin() =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(Email) &&
        !string.IsNullOrWhiteSpace(Password);

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var request = new LoginRequest
            {
                Email = Email.Trim(),
                Password = Password
            };

            var response = await _api.LoginAsync(request); // ◄ CORRIGIDO

            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                ErrorMessage = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => "Dados inválidos.",
                    HttpStatusCode.Unauthorized => "E-mail ou senha inválidos.",
                    HttpStatusCode.InternalServerError => "Erro no servidor.",
                    _ => "Erro ao realizar login."
                };

                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(response.Content.Token))
            {
                ErrorMessage = "Token inválido recebido.";
                HasError = true;
                return;
            }

            await _auth.SaveTokenAsync(response.Content.Token);

            // ✅ Rota absoluta correta
            await Shell.Current.GoToAsync("//home");
        }
        catch (ApiException apiEx)
        {
            HasError = true;
            ErrorMessage = "Erro ao comunicar com o servidor.";

            _logger.LogError(apiEx,
                "Erro HTTP no login | StatusCode: {StatusCode} | Content: {Content}",
                apiEx.StatusCode,
                apiEx.Content);
        }
        catch (HttpRequestException httpEx)
        {
            HasError = true;
            ErrorMessage = "Falha de conexão com a API.";

            _logger.LogError(httpEx, "Erro de rede no login");
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Erro inesperado.";

            _logger.LogError(ex,
                "Erro inesperado no login | Message: {Message} | Inner: {Inner}",
                ex.Message,
                ex.InnerException?.Message);
        }
        finally
        {
            IsBusy = false;
            LoginCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private Task GoToRegister() =>
        Shell.Current.GoToAsync("//register");

    [RelayCommand]
    private Task BiometricLogin() =>
        Application.Current!.MainPage!.DisplayAlert(
            "Biometria",
            "Biometria ainda não implementada",
            "OK");

    partial void OnEmailChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string value) => LoginCommand.NotifyCanExecuteChanged();
}
