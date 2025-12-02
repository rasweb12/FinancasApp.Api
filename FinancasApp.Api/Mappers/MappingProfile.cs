// FinancasApp.Api/Mappers/MappingProfile.cs
using AutoMapper;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;
using Umbraco.Cms.Core.Models.Membership;

namespace FinancasApp.Api.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Account
        CreateMap<Account, AccountDto>().ReverseMap();

        // Transaction
        CreateMap<Transaction, TransactionDto>().ReverseMap();

        // CreditCard
        CreateMap<CreditCard, CreditCardDto>().ReverseMap();

        // Invoice
        CreateMap<Invoice, InvoiceDto>().ReverseMap();

        // User (opcional, mas bom ter)
        // Comente ou remova temporariamente até criar o DTO
        //CreateMap<User, UserDto>().ReverseMap();

    }
}