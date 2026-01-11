using AutoMapper;
using FinancasApp.Api.DTOs;
using FinancasApp.Api.Models;

namespace FinancasApp.Api.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // =====================
        // ACCOUNT
        // =====================
        CreateMap<Account, AccountDto>().ReverseMap();

        // =====================
        // TRANSACTION
        // =====================
        CreateMap<Transaction, TransactionDto>().ReverseMap();

        // =====================
        // CREDIT CARD
        // =====================
        CreateMap<CreditCard, CreditCardDto>().ReverseMap();

        // =====================
        // INVOICE
        // =====================
        CreateMap<Invoice, InvoiceDto>().ReverseMap();

        // =====================
        // CATEGORY  ✅ (ERA O QUE FALTAVA)
        // =====================
        CreateMap<Category, CategoryDto>().ReverseMap();

        // =====================
        // USER (SE NECESSÁRIO)
        // =====================
        // CreateMap<User, UserDto>().ReverseMap();
    }
}
