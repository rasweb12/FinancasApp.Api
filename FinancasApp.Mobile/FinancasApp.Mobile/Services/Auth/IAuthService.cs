using System.Threading.Tasks;

namespace FinancasApp.Mobile.Services.Auth;

public class LoginResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }
}

public interface IAuthService
{
    string? Token { get; }

    /// <summary>
    /// Faz login e retorna um objeto com sucesso, mensagem e token
    /// </summary>
    Task<LoginResult> LoginAsync(string email, string password);

    /// <summary>
    /// Salva o token no SecureStorage
    /// </summary>
    Task SaveTokenAsync(string token);

    /// <summary>
    /// Carrega o token do SecureStorage na inicialização
    /// </summary>
    Task LoadTokenAsync();

    /// <summary>
    /// Remove o token (logout)
    /// </summary>
    Task LogoutAsync();
}