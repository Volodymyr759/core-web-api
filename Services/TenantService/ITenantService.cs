using System.Collections.Generic;

namespace CoreWebApi.Services.TenantService
{
    public interface ITenantService
    {
        IEnumerable<TenantDto> GetAllTenants();
        TenantDto GetTenantById(int id);
        TenantDto CreateTenant(CreateTenantDto createTenantDto);
        TenantDto UpdateTenant(TenantDto tenantDto);
        void DeleteTenant(TenantDto tenantDto);
    }
}
