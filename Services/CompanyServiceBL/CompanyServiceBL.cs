using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services
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

        public async Task<SearchResult<CompanyServiceDto>> GetCompanyServicesSearchResultAsync(int limit, int page, OrderType order)
        {
            Func<IQueryable<CompanyService>, IOrderedQueryable<CompanyService>> orderBy = null;// sorting only by Title
            orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);

            var services = await repository.GetAllAsync(null, orderBy);

            return new SearchResult<CompanyServiceDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)services.Count() / limit)),
                SearchCriteria = string.Empty,
                TotalItemCount = services.Count(),
                ItemList = (List<CompanyServiceDto>)mapper.Map<IEnumerable<CompanyServiceDto>>(services.Skip((page - 1) * limit).Take(limit))
            };
        }

        public async Task<CompanyServiceDto> GetCompanyServiceByIdAsync(int id) => mapper.Map<CompanyServiceDto>(await repository.GetAsync(id));

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
            var companyService = repository.Get(id);

            if (companyService != null)
            {
                companyService.IsActive = isActive;
                repository.Update(companyService);
            }
        }

        public async Task DeleteCompanyServiceAsync(int id) => await repository.DeleteAsync(id);
    }
}
