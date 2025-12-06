using System.Text.Json.Serialization;
using FinancasApp.Mobile.Models.DTOs;

namespace FinancasApp.Mobile.Models.DTOs;

/// <summary>
/// Resposta do endpoint /auth/register
/// Normalmente a API devolve os mesmos campos do login (token + userId)
/// + opcionalmente os dados do usuário recém-criado (fullName, email, etc)
/// </summary>
public class RegisterResponse : LoginResponse
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    // NUNCA devolva a senha no response! (segurança)
    // Removido intencionalmente — a API NÃO deve retornar a senha
    // [JsonPropertyName("password")] public string Password { get; set; } = string.Empty;
}