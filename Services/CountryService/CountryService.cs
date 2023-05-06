using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library;
using CoreWebApi.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class CountryService : AppBaseService<Country, CountryDto>, ICountryService
    {
        public CountryService(
            IMapper mapper,
            IRepository<Country> repository,
            ISearchResult<CountryDto> searchResult,
            IServiceResult<Country> serviceResult) : base(mapper, repository, searchResult, serviceResult) { }

        public async Task<ISearchResult<CountryDto>> GetAsync(int limit, int page, string sortField, OrderType order)
        {
            // sorting
            Func<IQueryable<Country>, IOrderedQueryable<Country>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Name) : orderBy = q => q.OrderByDescending(s => s.Name);

            return await Search(limit: limit, page: page, order: order, orderBy: orderBy);
        }

        public override async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await Repository.IsExistAsync("EXEC @returnVal=sp_checkCountryById @id, @returnVal", parameters);
        }
    }
}
