using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWebApi.Services.CompanyServiceBL
{
    public class CompanyServiceBL : ICompanyServiceBL
    {
        private readonly IMapper _mapper;
        private readonly IRepository<CompanyService> _repository;

        public CompanyServiceBL(IMapper mapper, IRepository<CompanyService> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public IEnumerable<CompanyServiceDto> GetAllCompanyServices(int page, string sort, int limit)
        {
            // sorting only by Title
            Func<IQueryable<CompanyService>, IOrderedQueryable<CompanyService>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);

            return _mapper.Map<IEnumerable<CompanyServiceDto>>(_repository.GetAll(limit, page, null, orderBy));
        }

        public CompanyServiceDto GetCompanyServiceById(int id)
        {
            return _mapper.Map<CompanyServiceDto>(_repository.Get(t => t.Id == id));
        }

        public CompanyServiceDto CreateCompanyService(CompanyServiceDto companyServiceDto)
        {
            var companyService = _mapper.Map<CompanyService>(companyServiceDto);

            return _mapper.Map<CompanyServiceDto>(_repository.Create(companyService));
        }

        public CompanyServiceDto UpdateCompanyService(CompanyServiceDto companyServiceDto)
        {
            var companyService = _mapper.Map<CompanyService>(companyServiceDto);

            return _mapper.Map<CompanyServiceDto>(_repository.Update(companyService));
        }

        public void SetIsActive(int id, bool isActive)
        {
            var companyService = _repository.Get(t => t.Id == id);

            if (companyService != null)
            {
                companyService.IsActive = isActive;
                _repository.Update(companyService);
            }
        }

        public CompanyServiceDto DeleteCompanyService(int id)
        {
            return _mapper.Map<CompanyServiceDto>(_repository.Delete(id));
        }
    }
}
