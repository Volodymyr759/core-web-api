using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services.EmployeeService
{
    public interface IEmployeeService
    {
        IEnumerable<EmployeeDto> GetAllEmployees(int limit, int page, string search, string sort_field, string sort);
        EmployeeDto GetEmployeeById(int id);
        EmployeeDto CreateEmployee(EmployeeDto employeeDto);
        EmployeeDto UpdateEmployee(EmployeeDto employeeDto);
        void DeleteEmployee(EmployeeDto employeeDto);
    }
}
