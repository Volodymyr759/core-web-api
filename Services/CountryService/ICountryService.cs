using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICountryService
    {
        Task<SearchResult<CountryDto>> GetCountriesSearchResultAsync(int limit, int page, OrderType order);
        CountryDto GetCountryById(int id);
        CountryDto CreateCountry(CountryDto countryDto);
        CountryDto UpdateCountry(CountryDto countryDto);
        CountryDto DeleteCountry(int id);
    }
}
