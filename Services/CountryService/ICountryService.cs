using CoreWebApi.Library;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICountryService : IBaseService<CountryDto>
    {
        Task<ISearchResult<CountryDto>> GetAsync(int limit, int page, string sortField, OrderType order);
    }
}
