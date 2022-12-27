using AutoMapper;
using CoreWebApi.Models;
using CoreWebApi.Services.CompanyServiceBL;
using CoreWebApi.Services.CountryService;
using CoreWebApi.Services.MailSubscriberService;
using CoreWebApi.Services.MailSubscriptionService;
using CoreWebApi.Services.TenantService;

namespace CoreWebApi.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CompanyService, CompanyServiceDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<MailSubscriber, MailSubscriberDto>().ReverseMap();
            CreateMap<MailSubscription, MailSubscriptionDto>().ReverseMap();
            CreateMap<Tenant, TenantDto>().ReverseMap();
            CreateMap<CreateTenantDto, Tenant>().ForMember(dest => dest.Id, act => act.Ignore());
        }
    }
}
