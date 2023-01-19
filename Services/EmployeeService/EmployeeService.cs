using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using CoreWebApi.Services.OfficeService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Employee> employeeRepository;
        private readonly IRepository<Office> officeRepository;

        public EmployeeService(
            IMapper mapper,
            IRepository<Employee> employeeRepository,
            IRepository<Office> officeRepository)
        {
            this.mapper = mapper;
            this.employeeRepository = employeeRepository;
            this.officeRepository = officeRepository;
        }

        public async Task<SearchResult<EmployeeDto>> GetEmployeesSearchResultAsync(int limit, int page, string search, string sort_field, OrderType order)
        {
            // search by FullName
            Expression<Func<Employee, bool>> searchQuery = null;
            if (search.Trim().Length > 0) searchQuery = t => t.FullName.Contains(search);

            // sorting - newest first
            Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy = null;
            orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Id) : orderBy = q => q.OrderByDescending(s => s.Id);

            var employees = await employeeRepository.GetAllAsync(searchQuery, orderBy);

            return new SearchResult<EmployeeDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)employees.Count() / limit)),
                SearchCriteria = search,
                TotalItemCount = employees.Count(),
                ItemList = (List<EmployeeDto>)mapper.Map<IEnumerable<EmployeeDto>>(employees.Skip((page - 1) * limit).Take(limit))
            };
        }

        public EmployeeDto GetEmployeeById(int id)
        {
            var employeeDto = mapper.Map<EmployeeDto>(employeeRepository.Get(id));
            if (employeeDto != null) employeeDto.OfficeDto = mapper.Map<OfficeDto>(officeRepository.Get(employeeDto.OfficeId));

            return employeeDto;
        }

        public EmployeeDto CreateEmployee(EmployeeDto employeeDto)
        {
            var employee = mapper.Map<Employee>(employeeDto);

            return mapper.Map<EmployeeDto>(employeeRepository.Create(employee));
        }

        public EmployeeDto UpdateEmployee(EmployeeDto employeeDto)
        {
            var employee = mapper.Map<Employee>(employeeDto);

            return mapper.Map<EmployeeDto>(employeeRepository.Update(employee));
        }

        public EmployeeDto DeleteEmployee(int id)
        {
            return mapper.Map<EmployeeDto>(employeeRepository.Delete(id));
        }
    }
}
