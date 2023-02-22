using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICountryService
    {
        Task<SearchResult<CountryDto>> GetCountriesSearchResultAsync(int limit, int page, OrderType order);
        Task<CountryDto> GetCountryByIdAsync(int id);
        Task<CountryDto> CreateCountryAsync(CountryDto countryDto);
        Task UpdateCountryAsync(CountryDto countryDto);
        Task DeleteCountryAsync(int id);
        Task<bool> IsExistAsync(int id);
    }
}
