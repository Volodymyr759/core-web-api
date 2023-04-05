using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IOfficeService : IBaseService<OfficeDto>
    {
        Task<SearchResult<OfficeDto>> GetOfficesSearchResultAsync(int limit, int page, string sortField, OrderType order);

        Task<List<OfficeNameIdDto>> GetOfficeIdNamesAsync();

        Task<List<OfficeDto>> GetOfficesByCountryId(int id);
    }
}
