﻿using AutoMapper;
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

            CreateMap<CreateTenantDto, Tenant>().ForMember(dest => dest.Id, act => act.Ignore());
        }
    }
}
