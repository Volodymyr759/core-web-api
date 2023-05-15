using CoreWebApi.Library;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICompanyServiceBL : IBaseService<CompanyServiceDto>
    {
        Task<ISearchResult<CompanyServiceDto>> GetAsync(int limit, int page, CompanyServiceStatus companyServiceStatus, string sortField, OrderType order);
    }
}
