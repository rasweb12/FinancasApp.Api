using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Services/IAuthService.cs
namespace FinancasApp.Mobile.Services.Auth;

public interface IAuthService
{
    string Token { get; }
    Task<bool> LoginAsync(string email, string password);
    Task SaveTokenAsync(string token);
    Task LoadTokenAsync();
    Task LogoutAsync();
}
