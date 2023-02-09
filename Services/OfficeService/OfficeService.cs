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
    public class OfficeService : IOfficeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Office> repository;
        private readonly IRepository<OfficeNameId> repositoryOfficeNameId;

        public OfficeService(
            IMapper mapper,
            IRepository<Office> repository,
            IRepository<OfficeNameId> repositoryOfficeNameId
            )
        {
            this.mapper = mapper;
            this.repository = repository;
            this.repositoryOfficeNameId = repositoryOfficeNameId;
        }

        public async Task<SearchResult<OfficeDto>> GetOfficesSearchResultAsync(int limit, int page, OrderType order)
        {
            // sorting sorting only by Name
            Func<IQueryable<Office>, IOrderedQueryable<Office>> orderBy = null;
            orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Name) : orderBy = q => q.OrderByDescending(s => s.Name);

            var offices = await repository.GetAllAsync(null, orderBy);

            return new SearchResult<OfficeDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)offices.Count() / limit)),
                SearchCriteria = string.Empty,
                TotalItemCount = offices.Count(),
                ItemList = (List<OfficeDto>)mapper.Map<IEnumerable<OfficeDto>>(offices.Skip((page - 1) * limit).Take(limit))
            };
        }

        public async Task<List<OfficeNameIdDto>> GetOfficeIdNamesAsync()
        {
            var officesNameIds = (List<OfficeNameIdDto>)mapper.Map<IEnumerable<OfficeNameIdDto>>(await repositoryOfficeNameId.GetAllAsync());

            return officesNameIds.OrderBy(o => o.Name).ToList<OfficeNameIdDto>();
        }

        public OfficeDto GetOfficeById(int id) => mapper.Map<OfficeDto>(repository.Get(id));

        public OfficeDto CreateOffice(OfficeDto officeDto)
        {
            var office = mapper.Map<Office>(officeDto);

            return mapper.Map<OfficeDto>(repository.Create(office));
        }

        public OfficeDto UpdateOffice(OfficeDto officeDto)
        {
            repository.Update(mapper.Map<Office>(officeDto));

            return officeDto;
        }

        public OfficeDto DeleteOffice(int id) => mapper.Map<OfficeDto>(repository.Delete(id));

    }
}
