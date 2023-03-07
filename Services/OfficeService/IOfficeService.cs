using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IOfficeService
    {
        Task<SearchResult<OfficeDto>> GetOfficesSearchResultAsync(int limit, int page, OrderType order);
        Task<List<OfficeNameIdDto>> GetOfficeIdNamesAsync();
        Task<OfficeDto> GetOfficeByIdAsync(int id);
        Task<List<OfficeDto>> GetOfficesByCountryId(int id);
        Task<OfficeDto> CreateOfficeAsync(OfficeDto officeDto);
        Task UpdateOfficeAsync(OfficeDto officeDto);
        Task DeleteOfficeAsync(int id);
        Task<bool> IsExistAsync(int id);
    }
}
