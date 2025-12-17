using System;

namespace FinancasApp.Api.DTOs;

public class CreditCardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Last4Digits { get; set; } = "0000";
    public decimal CreditLimit { get; set; }
    public int DueDay { get; set; }
    public int ClosingDay { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime UpdatedAt { get; set; }
}