using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWebApi.Services.CountryService
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

        public IEnumerable<CountryDto> GetAllCountries(int page, string sort, int limit = 10)
        {
            // sorting only by Name
            Func<IQueryable<Country>, IOrderedQueryable<Country>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Name) : orderBy = q => q.OrderByDescending(s => s.Name);

            return mapper.Map<IEnumerable<CountryDto>>(repository.GetAll(limit, page, null, orderBy));
        }

        public CountryDto GetCountryById(int id)
        {
            return mapper.Map<CountryDto>(repository.Get(t => t.Id == id));
        }

        public CountryDto CreateCountry(CountryDto countryDto)
        {
            var country = mapper.Map<Country>(countryDto);

            return mapper.Map<CountryDto>(repository.Create(country));
        }

        public CountryDto UpdateCountry(CountryDto countryDto)
        {
            var country = mapper.Map<Country>(countryDto);

            return mapper.Map<CountryDto>(repository.Update(country));
        }

        public CountryDto DeleteCountry(int id)
        {
            return mapper.Map<CountryDto>(repository.Delete(id));
        }
    }
}
