using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.EmployeeService;
using CoreWebApi.Services.VacancyService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWebApi.Services.OfficeService
{
    public class OfficeService : IOfficeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Office> officeRepository;
        private readonly IRepository<Employee> employeeRepository;
        private readonly IRepository<Vacancy> vacancyRepository;

        public OfficeService(
            IMapper mapper,
            IRepository<Office> officeRepository,
            IRepository<Employee> employeeRepository,
            IRepository<Vacancy> vacancyRepository)
        {
            this.mapper = mapper;
            this.officeRepository = officeRepository;
            this.employeeRepository = employeeRepository;
            this.vacancyRepository = vacancyRepository;
        }

        public IEnumerable<OfficeDto> GetAllOffices(int page, string sort, int limit)
        {
            // sorting only by Name
            Func<IQueryable<Office>, IOrderedQueryable<Office>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Name) : orderBy = q => q.OrderByDescending(s => s.Name);

            var officeDtos = mapper.Map<IEnumerable<OfficeDto>>(officeRepository.GetAll(limit, page, null, orderBy));

            if (((List<OfficeDto>)officeDtos).Count > 0)
            {
                foreach (var o in officeDtos)
                {
                    o.EmployeeDtos = mapper.Map<IEnumerable<EmployeeDto>>(employeeRepository.GetAll());
                    o.VacancyDtos = mapper.Map<IEnumerable<VacancyDto>>(vacancyRepository.GetAll());
                }
            }

            return officeDtos;
        }

        public OfficeDto GetOfficeById(int id)
        {
            var officeDto = mapper.Map<OfficeDto>(officeRepository.Get(id));
            if (officeDto != null)
            {
                officeDto.EmployeeDtos = mapper.Map<IEnumerable<EmployeeDto>>(employeeRepository.GetAll().Where(e => e.OfficeId == officeDto.Id));
                officeDto.VacancyDtos = mapper.Map<IEnumerable<VacancyDto>>(vacancyRepository.GetAll().Where(v => v.OfficeId == officeDto.Id));
            }

            return officeDto;
        }

        public OfficeDto CreateOffice(OfficeDto officeDto)
        {
            var office = mapper.Map<Office>(officeDto);

            return mapper.Map<OfficeDto>(officeRepository.Create(office));
        }

        public OfficeDto UpdateOffice(OfficeDto officeDto)
        {
            var office = mapper.Map<Office>(officeDto);

            return mapper.Map<OfficeDto>(officeRepository.Update(office));
        }

        public OfficeDto DeleteOffice(int id)
        {
            return mapper.Map<OfficeDto>(officeRepository.Delete(id));
        }
    }
}
