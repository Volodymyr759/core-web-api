using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services.OfficeService
{
    public interface IOfficeService
    {
        IEnumerable<OfficeDto> GetAllOffices(int limit, int page, string search, string sort_field, string sort);
        OfficeDto GetOfficeById(int id);
        OfficeDto CreateOffice(OfficeDto officeDto);
        OfficeDto UpdateOffice(OfficeDto officeDto);
        void DeleteOffice(OfficeDto officeDto);
    }
}
