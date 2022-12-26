using System.Collections.Generic;

namespace CoreWebApi.Services.CompanyServiceBL
{
    public interface ICompanyServiceBL
    {
        IEnumerable<CompanyServiceDto> GetAllCompanyServices(int page, string sort, int limit);
        CompanyServiceDto GetCompanyServiceById(int id);
        CompanyServiceDto CreateCompanyService(CompanyServiceDto companyServiceDto);
        CompanyServiceDto UpdateCompanyService(CompanyServiceDto companyServiceDto);
        CompanyServiceDto DeleteCompanyService(int id);
        void SetIsActive(int id, bool isActive);
    }
}
