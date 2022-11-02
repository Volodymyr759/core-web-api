using AutoMapper;
using CoreWebApi.Models.Tenant;
using CoreWebApi.Services.TenantService;

namespace CoreWebApi.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Tenant, TenantDto>().ReverseMap();
            CreateMap<CreateTenantDto, Tenant>().ForMember(dest => dest.Id, act => act.Ignore());
        }
    }
}
