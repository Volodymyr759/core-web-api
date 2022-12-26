using System.Collections.Generic;

namespace CoreWebApi.Services.CountryService
{
    public interface ICountryService
    {
        IEnumerable<CountryDto> GetAllCountries(int page, string sort, int limit);
        CountryDto GetCountryById(int id);
        CountryDto CreateCountry(CountryDto countryDto);
        CountryDto UpdateCountry(CountryDto countryDto);
        CountryDto DeleteCountry(int id);
    }
}
