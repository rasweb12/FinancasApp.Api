// Models/DTOs/LoginResponse.cs
using System.Text.Json.Serialization;

namespace FinancasApp.Mobile.Models.DTOs;

public class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
}