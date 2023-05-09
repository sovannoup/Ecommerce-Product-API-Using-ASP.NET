using AutoMapper;
using LicenseKey.Controllers.Request;
using LicenseKey.Helpers.Dto;
using LicenseKey.Hubs;
using LicenseKey.Models;

namespace LicenseKey.Helpers.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile() {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<CreateUserReq, User>();

            CreateMap<PaymentDto, Payment>();
            CreateMap<Payment, PaymentDto>();

            CreateMap<UserConnection, ContactMessage>();
        }
    }
}
