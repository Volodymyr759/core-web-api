using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
            if (order != OrderType.None)
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

        public async Task<OfficeDto> GetOfficeByIdAsync(int id) => mapper.Map<OfficeDto>(await repository.GetAsync(id));

        public async Task<List<OfficeDto>> GetOfficesByCountryId(int id)
        {
            var offices = mapper.Map<IEnumerable<OfficeDto>>(await repository.GetAllAsync()).ToList();

            return offices.FindAll(office => office.CountryId == id);
        }

        public async Task<OfficeDto> CreateOfficeAsync(OfficeDto officeDto)
        {
            var office = mapper.Map<Office>(officeDto);

            return mapper.Map<OfficeDto>(await repository.CreateAsync(office));
        }

        public async Task UpdateOfficeAsync(OfficeDto officeDto) => await repository.UpdateAsync(mapper.Map<Office>(officeDto));

        public async Task DeleteOfficeAsync(int id) => await repository.DeleteAsync(id);

        public async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await repository.IsExistAsync("EXEC @returnVal=sp_sp_checkOfficeById @id, @returnVal", parameters);
        }

    }
}
