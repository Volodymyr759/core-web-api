using AutoMapper;
using CoreWebApi.Models;

namespace CoreWebApi.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Candidate, CandidateDto>()
                .ForMember(dest => dest.VacancyDto, act => act.MapFrom(src => new VacancyDto()
                {
                    Id = src.Vacancy.Id,
                    Title = src.Vacancy.Title,
                    IsActive = src.Vacancy.IsActive
                }));

            CreateMap<CandidateDto, Candidate>();

            CreateMap<CompanyService, CompanyServiceDto>().ReverseMap();

            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.OfficeDtos, act => act.MapFrom(src => src.Offices));
            CreateMap<CountryDto, Country>();

            CreateMap<Office, OfficeDto>()
                .ForMember(dest => dest.VacancyDtos, act => act.MapFrom(src => src.Vacancies));
            CreateMap<OfficeDto, Office>();
            CreateMap<OfficeNameId, OfficeNameIdDto>();

            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.OfficeDto, act => act.MapFrom(src => src.Office));
            CreateMap<EmployeeDto, Employee>();

            CreateMap<MailSubscriber, MailSubscriberDto>().ReverseMap();

            CreateMap<MailSubscription, MailSubscriptionDto>()
                .ForMember(dest => dest.MailSubscriberDtos, act => act.MapFrom(src => src.MailSubscribers)); ;
            CreateMap<MailSubscriptionDto, MailSubscription>();

            CreateMap<Tenant, TenantDto>().ReverseMap();

            CreateMap<Vacancy, VacancyDto>()
                .ForMember(dest => dest.CandidateDtos, act => act.MapFrom(src => src.Candidates))
                .ForMember(dest => dest.OfficeDto, act => act.MapFrom(src => new OfficeDto()
                {
                    Id = src.OfficeId,
                    Name = src.Office.Name,
                    Address = src.Office.Address
                }));

            CreateMap<VacancyDto, Vacancy>();
            CreateMap<VacancyTitleId, VacancyTitleIdDto>();

            CreateMap<CreateTenantDto, Tenant>().ForMember(dest => dest.Id, act => act.Ignore());
        }
    }
}
