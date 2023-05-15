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
    public class CompanyServiceBL : AppBaseService<CompanyService, CompanyServiceDto>, ICompanyServiceBL
    {
        public CompanyServiceBL(
            IMapper mapper,
            IRepository<CompanyService> repository,
            ISearchResult<CompanyServiceDto> searchResult,
            IServiceResult<CompanyService> serviceResult) : base(mapper, repository, searchResult, serviceResult) { }

        public async Task<ISearchResult<CompanyServiceDto>> GetAsync(int limit, int page, CompanyServiceStatus companyServiceStatus, string sortField, OrderType order)
        {
            // filtering
            var filters = new List<Expression<Func<CompanyService, bool>>>();
            if (companyServiceStatus == CompanyServiceStatus.Active) filters.Add(v => v.IsActive == true);
            if (companyServiceStatus == CompanyServiceStatus.Disabled) filters.Add(v => v.IsActive == false);

            // sorting
            Func<IQueryable<CompanyService>, IOrderedQueryable<CompanyService>> orderBy = null;
            if (order != OrderType.None)
            {
                orderBy = sortField switch
                {
                    "Description" => order == OrderType.Ascending ? q => q.OrderBy(s => s.Description) : orderBy = q => q.OrderByDescending(s => s.Description),
                    _ => order == OrderType.Ascending ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title),
                };
            }

            return await Search(limit: limit, page: page, filters: filters, order: order, orderBy: orderBy);
        }

        public override async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await Repository.IsExistAsync("EXEC @returnVal=sp_checkCompanyServiceById @id, @returnVal", parameters);
        }
    }
}
