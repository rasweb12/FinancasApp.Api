using System.Threading.Tasks;

namespace FinancasApp.Mobile.Services.Auth;

public class LoginResult
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public string? Token { get; init; }

    public static LoginResult Ok(string token)
        => new()
        {
            Success = true,
            Token = token
        };

    public static LoginResult Error(string message)
        => new()
        {
            Success = false,
            Message = message
        };
}



public interface IAuthService
{
    string? Token { get; }

    Task<LoginResult> LoginAsync(string email, string password);
    Task SaveTokenAsync(string token);
    Task LoadTokenAsync();
    Task<string?> GetTokenAsync();
    Task LogoutAsync();
}
