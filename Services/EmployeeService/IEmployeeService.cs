using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services.EmployeeService
{
    public interface IEmployeeService
    {
        Task<SearchResult<EmployeeDto>> GetEmployeesSearchResultAsync(int limit, int page, string search, string sort_field, OrderType order);
        EmployeeDto GetEmployeeById(int id);
        EmployeeDto CreateEmployee(EmployeeDto employeeDto);
        EmployeeDto UpdateEmployee(EmployeeDto employeeDto);
        EmployeeDto DeleteEmployee(int id);
    }
}
