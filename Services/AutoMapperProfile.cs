using AutoMapper;
using CoreWebApi.Models;
using CoreWebApi.Models.Account;

namespace CoreWebApi.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>();

            CreateMap<Candidate, CandidateDto>()
                .ForMember(dest => dest.VacancyDto, act => act.MapFrom(src => new VacancyDto()
                {
                    Id = src.Vacancy.Id,
                    Title = src.Vacancy.Title,
                    Description = src.Vacancy.Description,
                    Previews = src.Vacancy.Previews,
                    IsActive = src.Vacancy.IsActive,
                    OfficeId = src.Vacancy.OfficeId,
                    OfficeDto = new OfficeDto()
                    {
                        Name = src.Vacancy.Office.Name,
                        Address = src.Vacancy.Office.Address
                    }
                }));

            CreateMap<CandidateDto, Candidate>();

            CreateMap<CompanyService, CompanyServiceDto>().ReverseMap();

            CreateMap<Country, CountryDto>()
                .ForMember(dest => dest.OfficeDtos, act => act.MapFrom(src => src.Offices));
            CreateMap<CountryDto, Country>();

            CreateMap<FileModel, FileModelDto>().ReverseMap();

            CreateMap<Office, OfficeDto>()
                .ForMember(dest => dest.CountryDto, act => act.MapFrom(src => new CountryDto()
                {
                    Id = src.CountryId,
                    Name = src.Country.Name,
                    Code = src.Country.Code
                }))
                .ForMember(dest => dest.EmployeeDtos, opt => opt.Ignore())
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

            CreateMap<Vacancy, VacancyDto>()
                .ForMember(dest => dest.OfficeDto, act => act.MapFrom(src => new OfficeDto()
                {
                    Id = src.OfficeId,
                    Name = src.Office.Name,
                    Description = src.Office.Description,
                    Address = src.Office.Address
                }));

            CreateMap<VacancyDto, Vacancy>()
                .ForMember(dest => dest.Office, act => act.Ignore())
                .ForMember(dest => dest.Candidates, act => act.Ignore());
        }
    }
}
