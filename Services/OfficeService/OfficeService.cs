using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services.OfficeService
{
    public class OfficeService : IOfficeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Office> repository;

        public OfficeService(IMapper mapper, IRepository<Office> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public IEnumerable<OfficeDto> GetAllOffices(int limit, int page, string search, string sort_field, string sort)
        {
            throw new NotImplementedException();
        }

        public OfficeDto GetOfficeById(int id)
        {
            throw new NotImplementedException();
        }

        public OfficeDto CreateOffice(OfficeDto officeDto)
        {
            throw new NotImplementedException();
        }

        public OfficeDto UpdateOffice(OfficeDto officeDto)
        {
            throw new NotImplementedException();
        }

        public OfficeDto DeleteOffice(int id)
        {
            throw new NotImplementedException();
        }
    }
}
