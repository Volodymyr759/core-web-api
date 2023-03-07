using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Employee> repository;

        public EmployeeService(
            IMapper mapper,
            IRepository<Employee> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public async Task<SearchResult<EmployeeDto>> GetEmployeesSearchResultAsync(int limit, int page, string search, string sortField, OrderType order)
        {
            // search by FullName
            Expression<Func<Employee, bool>> searchQuery = null;
            if (!string.IsNullOrEmpty(search)) searchQuery = t => t.FullName.Contains(search);

            // sorting only by FullName
            Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.FullName) : orderBy = q => q.OrderByDescending(s => s.FullName);

            var employees = await repository.GetAllAsync(searchQuery, orderBy);

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

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int id) => mapper.Map<EmployeeDto>(await repository.GetAsync(id));

        public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto employeeDto)
        {
            var employee = mapper.Map<Employee>(employeeDto);

            return mapper.Map<EmployeeDto>(await repository.CreateAsync(employee));
        }

        public async Task UpdateEmployeeAsync(EmployeeDto employeeDto) =>
            await repository.UpdateAsync(mapper.Map<Employee>(employeeDto));

        public async Task DeleteEmployeeAsync(int id) => await repository.DeleteAsync(id);

        public async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await repository.IsExistAsync("EXEC @returnVal=sp_checkEmployeeById @id, @returnVal", parameters);
        }
    }
}
