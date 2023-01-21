using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IOfficeService
    {
        Task<SearchResult<OfficeDto>> GetOfficesSearchResultAsync(int limit, int page, OrderType order);
        OfficeDto GetOfficeById(int id);
        OfficeDto CreateOffice(OfficeDto officeDto);
        OfficeDto UpdateOffice(OfficeDto officeDto);
        OfficeDto DeleteOffice(int id);
    }
}
