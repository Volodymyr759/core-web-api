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

            // sorting by FullName, Position or Description
            Func<IQueryable<Employee>, IOrderedQueryable<Employee>> orderBy = null;
            if (order != OrderType.None)
            {
                orderBy = sortField switch
                {
                    "Position" => order == OrderType.Ascending ? q => q.OrderBy(e => e.Position) : orderBy = q => q.OrderByDescending(e => e.Position),
                    "Description" => order == OrderType.Ascending ? q => q.OrderBy(e => e.Description) : orderBy = q => q.OrderByDescending(e => e.Description),
                    _ => order == OrderType.Ascending ? q => q.OrderBy(e => e.FullName) : orderBy = q => q.OrderByDescending(e => e.FullName),
                };
            }

            // adding navigation properties
            Expression<Func<Employee, object>> includeOffice = e => e.Office;
            Expression<Func<Employee, object>>[] navProperties =
                new Expression<Func<Employee, object>>[] { includeOffice };

            return await Search(limit: limit, page: page,
                search: search, filters: filters, order: order, orderBy: orderBy, navigationProperties: navProperties);
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
