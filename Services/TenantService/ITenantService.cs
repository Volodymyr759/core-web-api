using System.Collections.Generic;

namespace CoreWebApi.Services
{
    public interface ITenantService
    {
        IEnumerable<TenantDto> GetAllTenants(int limit, int page, string search, string sort_field, string sort);
        TenantDto GetTenantById(int id);
        TenantDto CreateTenant(CreateTenantDto createTenantDto);
        TenantDto UpdateTenant(TenantDto tenantDto);
        void DeleteTenant(TenantDto tenantDto);
    }
}
