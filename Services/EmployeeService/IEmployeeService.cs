using CoreWebApi.Library;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IEmployeeService : IBaseService<EmployeeDto>
    {
        Task<ISearchResult<EmployeeDto>> GetAsync(int limit, int page, string search, int? officeId, string sortField, OrderType order);
    }
}
