using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICompanyServiceBL : IBaseService<CompanyServiceDto>
    {
        Task<SearchResult<CompanyServiceDto>> GetCompanyServicesSearchResultAsync(int limit, int page, CompanyServiceStatus companyServiceStatus, OrderType order);
    }
}
