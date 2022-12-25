using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services.ContryService
{
    public interface ICountryService
    {
        IEnumerable<CountryDto> GetAllCountries(int limit, string sort_field, string sort);
        CountryDto GetCountryById(int id);
        CountryDto CreateCountry(CountryDto countryDto);
        CountryDto UpdateCountry(CountryDto countryDto);
        void DeleteCountry(CountryDto countryDto);
    }
}
