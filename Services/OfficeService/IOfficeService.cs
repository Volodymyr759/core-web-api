using CoreWebApi.Library;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IOfficeService : IBaseService<OfficeDto>
    {
        Task<ISearchResult<OfficeDto>> GetAsync(int limit, int page, string sortField, OrderType order);
        Task<List<OfficeNameIdDto>> GetOfficeIdNamesAsync();
    }
}
