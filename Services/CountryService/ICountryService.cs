using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICountryService : IBaseService<CountryDto>
    {
        Task<SearchResult<CountryDto>> GetCountriesSearchResultAsync(int limit, int page, string sortField, OrderType order);
    }
}
