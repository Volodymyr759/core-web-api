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
        private readonly IMapper mapper;
        private readonly IRepository<CompanyService> repository;

        public CompanyServiceBL(IMapper mapper, IRepository<CompanyService> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public IEnumerable<CompanyServiceDto> GetAllCompanyServices(int page, string sort, int limit)
        {
            // sorting only by Title
            Func<IQueryable<CompanyService>, IOrderedQueryable<CompanyService>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);

            return mapper.Map<IEnumerable<CompanyServiceDto>>(repository.GetAll(limit, page, null, orderBy));
        }

        public CompanyServiceDto GetCompanyServiceById(int id)
        {
            return mapper.Map<CompanyServiceDto>(repository.Get(t => t.Id == id));
        }

        public CompanyServiceDto CreateCompanyService(CompanyServiceDto companyServiceDto)
        {
            var companyService = mapper.Map<CompanyService>(companyServiceDto);

            return mapper.Map<CompanyServiceDto>(repository.Create(companyService));
        }

        public CompanyServiceDto UpdateCompanyService(CompanyServiceDto companyServiceDto)
        {
            var companyService = mapper.Map<CompanyService>(companyServiceDto);

            return mapper.Map<CompanyServiceDto>(repository.Update(companyService));
        }

        public void SetIsActive(int id, bool isActive)
        {
            var companyService = repository.Get(t => t.Id == id);

            if (companyService != null)
            {
                companyService.IsActive = isActive;
                repository.Update(companyService);
            }
        }

        public CompanyServiceDto DeleteCompanyService(int id)
        {
            return mapper.Map<CompanyServiceDto>(repository.Delete(id));
        }
    }
}
