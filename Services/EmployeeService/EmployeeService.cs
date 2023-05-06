using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library;
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
    public class EmployeeService : AppBaseService<Employee, EmployeeDto>, IEmployeeService
    {
        public EmployeeService(
            IMapper mapper,
            IRepository<Employee> repository,
            ISearchResult<EmployeeDto> searchResult,
            IServiceResult<Employee> serviceResult) : base(mapper, repository, searchResult, serviceResult) { }

        public async Task<ISearchResult<EmployeeDto>> GetAsync(int limit, int page, string search, string sortField, OrderType order)
        {
            // filtering
            var filters = new List<Expression<Func<Employee, bool>>>();
            if (!string.IsNullOrEmpty(search)) filters.Add(t => t.FullName.Contains(search));

            // sorting
            Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.FullName) : orderBy = q => q.OrderByDescending(s => s.FullName);

            return await Search(limit: limit, page: page, search: search, filters: filters, order: order, orderBy: orderBy);
        }

        public override async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await Repository.IsExistAsync("EXEC @returnVal=sp_checkEmployeeById @id, @returnVal", parameters);
        }
    }
}
