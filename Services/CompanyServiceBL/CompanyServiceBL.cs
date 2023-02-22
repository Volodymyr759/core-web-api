using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<CompanyServiceDto> CreateCompanyServiceAsync(CompanyServiceDto companyServiceDto)
        {
            var companyService = mapper.Map<CompanyService>(companyServiceDto);

            return mapper.Map<CompanyServiceDto>(await repository.CreateAsync(companyService));
        }

        public async Task UpdateCompanyServiceAsync(CompanyServiceDto companyServiceDto) =>
            await repository.UpdateAsync(mapper.Map<CompanyService>(companyServiceDto));

        public async Task<CompanyServiceDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument)
        {
            var service = await repository.GetAsync(id);
            patchDocument.ApplyTo(service);
            return mapper.Map<CompanyServiceDto>(await repository.SaveAsync(service));
        }

        public async Task DeleteCompanyServiceAsync(int id) => await repository.DeleteAsync(id);

        public async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await repository.IsExistAsync("EXEC @returnVal=sp_checkCompanyServiceById @id, @returnVal", parameters);
        }
    }
}
