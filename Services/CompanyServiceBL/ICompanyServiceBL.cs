using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICompanyServiceBL
    {
        Task<SearchResult<CompanyServiceDto>> GetCompanyServicesSearchResultAsync(int limit, int page, OrderType order);
        Task<CompanyServiceDto> GetCompanyServiceByIdAsync(int id);
        Task<CompanyServiceDto> CreateCompanyServiceAsync(CompanyServiceDto companyServiceDto);
        Task UpdateCompanyServiceAsync(CompanyServiceDto companyServiceDto);
        Task DeleteCompanyServiceAsync(int id);
        void SetIsActive(int id, bool isActive);
        Task<bool> IsExistAsync(int id);
    }
}
