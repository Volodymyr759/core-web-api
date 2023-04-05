using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IEmployeeService : IBaseService<EmployeeDto>
    {
        Task<SearchResult<EmployeeDto>> GetEmployeesSearchResultAsync(int limit, int page, string search, string sortField, OrderType order);
    }
}
