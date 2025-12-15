// Controllers/UsersController.cs — VERSÃO FINAL E IMORTAL (.NET 9 – 06/12/2025)
using FinancasApp.Api.Data;
using FinancasApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinancasApp.Api.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous] // ← Permite login e register sem JWT
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<UsersController> _logger;

    public UsersController(AppDbContext context, IConfiguration config, ILogger<UsersController> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    // Em Register e Login
    //_logger.LogInformation("Tentativa de register: {Email}", dto.Email);
     // Ou em erro
    //_logger.LogWarning("Falha no login para {Email}", email);

    // POST /auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "E-mail e senha são obrigatórios" });

        dto.Email = dto.Email.Trim().ToLowerInvariant();

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "E-mail já cadastrado" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FullName = dto.FullName?.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Usuário criado com sucesso!",
            user = new { user.Id, user.Email, user.FullName }
        });
    }

    // POST /auth/login ← É AQUI QUE O MOBILE CHAMA
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { message = "E-mail e senha são obrigatórios" });

        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "E-mail ou senha inválidos" });

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            token,
            expiresIn = 604800, // 7 dias em segundos — compatível com o mobile
            user = new
            {
                id = user.Id,
                email = user.Email,
                fullName = user.FullName
            }
        });
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName ?? string.Empty)
        };

        var keyString = _config["Jwt:Key"]
            ?? "minha-chave-super-secreta-de-32-caracteres-1234567890";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "FinancasAppApi",
            audience: _config["Jwt:Audience"] ?? "FinancasAppMobile",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// DTOs — fora da classe pra não dar conflito
public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FullName { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}