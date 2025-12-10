using System.Text.Json.Serialization;

namespace FinancasApp.Mobile.Models.DTOs;

/// <summary>
/// Resposta do endpoint /auth/register
/// Inclui token + userId (herdado) + dados do usuário criado
/// </summary>
public class RegisterResponse : LoginResponse
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    // NUNCA inclua senha aqui!
}