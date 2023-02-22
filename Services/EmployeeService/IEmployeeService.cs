using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IEmployeeService
    {
        Task<SearchResult<EmployeeDto>> GetEmployeesSearchResultAsync(int limit, int page, string search, string sort_field, OrderType order);
        Task<EmployeeDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto);
        Task UpdateEmployeeAsync(EmployeeDto employeeDto);
        Task DeleteEmployeeAsync(int id);
        Task<bool> IsExistAsync(int id);
    }
}
