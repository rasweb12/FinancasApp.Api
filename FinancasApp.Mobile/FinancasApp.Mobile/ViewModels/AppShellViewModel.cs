// ViewModels/AppShellViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FinancasApp.Mobile.Services.Auth;

namespace FinancasApp.Mobile.ViewModels;

public partial class AppShellViewModel : ObservableObject,
    IRecipient<LoginSuccessMessage>,
    IRecipient<LogoutMessage>
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private bool isAuthenticated;

    public AppShellViewModel(IAuthService authService)
    {
        _authService = authService;

        // Registra mensagens de forma segura
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    /// <summary>
    /// Chamado pelo AppShell quando o Shell aparece
    /// </summary>
    public async Task InitializeAsync()
    {
        var token = await _authService.GetTokenAsync();
        IsAuthenticated = !string.IsNullOrWhiteSpace(token);
    }

    // 🔐 Login
    public void Receive(LoginSuccessMessage message)
    {
        IsAuthenticated = true;
    }

    // 🔓 Logout
    public void Receive(LogoutMessage message)
    {
        IsAuthenticated = false;
    }
}
