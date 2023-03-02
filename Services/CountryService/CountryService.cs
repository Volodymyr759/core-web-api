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
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class CountryService : ICountryService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Country> repository;

        public CountryService(IMapper mapper, IRepository<Country> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public async Task<SearchResult<CountryDto>> GetCountriesSearchResultAsync(int limit, int page, OrderType order)
        {
            // sorting only by Name
            Func<IQueryable<Country>, IOrderedQueryable<Country>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Name) : orderBy = q => q.OrderByDescending(s => s.Name);

            var countries = await repository.GetAllAsync(null, orderBy);

            return new SearchResult<CountryDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)countries.Count() / limit)),
                SearchCriteria = "",
                TotalItemCount = countries.Count(),
                ItemList = (List<CountryDto>)mapper.Map<IEnumerable<CountryDto>>(countries.Skip((page - 1) * limit).Take(limit))
            };
        }

        public async Task<CountryDto> GetCountryByIdAsync(int id) => mapper.Map<CountryDto>(await repository.GetAsync(id));

        public async Task<CountryDto> CreateCountryAsync(CountryDto countryDto)
        {
            var country = mapper.Map<Country>(countryDto);

            return mapper.Map<CountryDto>(await repository.CreateAsync(country));
        }

        public async Task UpdateCountryAsync(CountryDto countryDto) =>
            await repository.UpdateAsync(mapper.Map<Country>(countryDto));

        public async Task DeleteCountryAsync(int id) => await repository.DeleteAsync(id);

        public async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await repository.IsExistAsync("EXEC @returnVal=sp_checkCountryById @id, @returnVal", parameters);
        }
    }
}
