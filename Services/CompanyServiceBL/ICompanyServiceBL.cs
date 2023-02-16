using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICompanyServiceBL
    {
        Task<SearchResult<CompanyServiceDto>> GetCompanyServicesSearchResultAsync(int limit, int page, OrderType order);
        Task<CompanyServiceDto> GetCompanyServiceByIdAsync(int id);
        CompanyServiceDto CreateCompanyService(CompanyServiceDto companyServiceDto);
        CompanyServiceDto UpdateCompanyService(CompanyServiceDto companyServiceDto);
        Task DeleteCompanyServiceAsync(int id);
        void SetIsActive(int id, bool isActive);
    }
}
