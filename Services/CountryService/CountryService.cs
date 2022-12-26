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
        private readonly IMapper _mapper;
        private readonly IRepository<Country> _repository;

        public CountryService(IMapper mapper, IRepository<Country> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<CountryDto> GetAllCountries(int page, string sort, int limit = 10)
        {
            // sorting only by Name
            Func<IQueryable<Country>, IOrderedQueryable<Country>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Name) : orderBy = q => q.OrderByDescending(s => s.Name);

            return _mapper.Map<IEnumerable<CountryDto>>(_repository.GetAll(limit, page, null, orderBy));
        }

        public CountryDto GetCountryById(int id)
        {
            return _mapper.Map<CountryDto>(_repository.Get(t => t.Id == id));
        }

        public CountryDto CreateCountry(CountryDto countryDto)
        {
            var country = _mapper.Map<Country>(countryDto);

            return _mapper.Map<CountryDto>(_repository.Create(country));
        }

        public CountryDto UpdateCountry(CountryDto countryDto)
        {
            var country = _mapper.Map<Country>(countryDto);

            return _mapper.Map<CountryDto>(_repository.Update(country));
        }

        public CountryDto DeleteCountry(int id)
        {
            return _mapper.Map<CountryDto>(_repository.Delete(id));
        }
    }
}
