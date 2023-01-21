using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
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

        public CountryDto GetCountryById(int id) => mapper.Map<CountryDto>(repository.Get(id));

        public CountryDto CreateCountry(CountryDto countryDto)
        {
            var country = mapper.Map<Country>(countryDto);

            return mapper.Map<CountryDto>(repository.Create(country));
        }

        public CountryDto UpdateCountry(CountryDto countryDto)
        {
            repository.Update(mapper.Map<Country>(countryDto));
            return countryDto;
        }

        public CountryDto DeleteCountry(int id) => mapper.Map<CountryDto>(repository.Delete(id));
    }
}
