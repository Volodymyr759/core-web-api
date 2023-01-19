using AutoMapper;
using CoreWebApi.Models;
using CoreWebApi.Services.CandidateService;
using CoreWebApi.Services.CompanyServiceBL;
using CoreWebApi.Services.CountryService;
using CoreWebApi.Services.EmployeeService;
using CoreWebApi.Services.MailSubscriberService;
using CoreWebApi.Services.MailSubscriptionService;
using CoreWebApi.Services.OfficeService;
using CoreWebApi.Services.TenantService;
using CoreWebApi.Services.VacancyService;

namespace CoreWebApi.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Candidate, CandidateDto>().ReverseMap();
            CreateMap<CompanyService, CompanyServiceDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Office, OfficeDto>().ReverseMap();
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.OfficeDto, act => act.MapFrom(src => src.Office));
            CreateMap<EmployeeDto, Employee>();
            CreateMap<MailSubscriber, MailSubscriberDto>().ReverseMap();
            CreateMap<MailSubscription, MailSubscriptionDto>().ReverseMap();
            CreateMap<Tenant, TenantDto>().ReverseMap();
            CreateMap<Vacancy, VacancyDto>().ReverseMap();
            CreateMap<CreateTenantDto, Tenant>().ForMember(dest => dest.Id, act => act.Ignore());
        }
    }
}
