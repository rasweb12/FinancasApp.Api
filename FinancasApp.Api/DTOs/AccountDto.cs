// DTOs/AccountDto.cs
using System.ComponentModel.DataAnnotations;

namespace FinancasApp.Api.DTOs;

// Enum FORA da classe (resolve ambiguidade)
public enum AccountTypeDto
{
    Checking = 1,
    Savings = 2,
    Wallet = 3,
    Investment = 4,
    Credit = 5
}

public class AccountDto
{
    public Guid Id { get; set; }
    [Required][StringLength(100)] public string Name { get; set; } = string.Empty;
    public AccountTypeDto AccountType { get; set; } = AccountTypeDto.Checking;
    public string Currency { get; set; } = "BRL";
    public decimal Balance { get; set; }
    public decimal InitialBalance { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}