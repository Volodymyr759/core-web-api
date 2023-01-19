using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services.CompanyServiceBL
{
    public interface ICompanyServiceBL
    {
        Task<SearchResult<CompanyServiceDto>> GetCompanyServicesSearchResultAsync(int limit, int page, OrderType order);
        CompanyServiceDto GetCompanyServiceById(int id);
        CompanyServiceDto CreateCompanyService(CompanyServiceDto companyServiceDto);
        CompanyServiceDto UpdateCompanyService(CompanyServiceDto companyServiceDto);
        CompanyServiceDto DeleteCompanyService(int id);
        void SetIsActive(int id, bool isActive);
    }
}
