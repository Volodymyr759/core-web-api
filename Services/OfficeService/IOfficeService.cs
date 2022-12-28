using System.Collections.Generic;

namespace CoreWebApi.Services.OfficeService
{
    public interface IOfficeService
    {
        IEnumerable<OfficeDto> GetAllOffices(int page, string sort, int limit);
        OfficeDto GetOfficeById(int id);
        OfficeDto CreateOffice(OfficeDto officeDto);
        OfficeDto UpdateOffice(OfficeDto officeDto);
        OfficeDto DeleteOffice(int id);
    }
}
