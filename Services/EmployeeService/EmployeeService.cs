using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.OfficeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public IEnumerable<EmployeeDto> GetAllEmployees(int limit, int page, string search, string sort_field, string sort)
        {
            // search by FullName
            Expression<Func<Employee, bool>> searchQuery = null;
            if (search.Trim().Length > 0) searchQuery = t => t.FullName.Contains(search);

            // sorting - newest first
            Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Id) : orderBy = q => q.OrderByDescending(s => s.Id);

            return mapper.Map<IEnumerable<EmployeeDto>>(employeeRepository.GetAll(limit, page, searchQuery, orderBy));
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
