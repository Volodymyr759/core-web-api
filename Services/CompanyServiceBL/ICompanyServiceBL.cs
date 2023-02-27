using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICompanyServiceBL
    {
        Task<SearchResult<CompanyServiceDto>> GetCompanyServicesSearchResultAsync(int limit, int page, CompanyServiceStatus companyServiceStatus, OrderType order);
        Task<CompanyServiceDto> GetCompanyServiceByIdAsync(int id);
        Task<CompanyServiceDto> CreateCompanyServiceAsync(CompanyServiceDto companyServiceDto);
        Task UpdateCompanyServiceAsync(CompanyServiceDto companyServiceDto);
        Task DeleteCompanyServiceAsync(int id);
        Task<CompanyServiceDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument);
        Task<bool> IsExistAsync(int id);
    }
}
