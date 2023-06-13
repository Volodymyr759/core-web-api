using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library;
using CoreWebApi.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class OfficeService : AppBaseService<Office, OfficeDto>, IOfficeService
    {
        private readonly IRepository<OfficeNameId> repositoryOfficeNameId;

        public OfficeService(
            IMapper mapper,
            IRepository<Office> repository,
            IRepository<OfficeNameId> repositoryOfficeNameId,
            ISearchResult<OfficeDto> searchResult,
            IServiceResult<Office> serviceResult) : base(mapper, repository, searchResult, serviceResult)
        {
            this.repositoryOfficeNameId = repositoryOfficeNameId;
        }

        public async Task<ISearchResult<OfficeDto>> GetAsync(int limit, int page, string sortField, OrderType order)
        {
            // sorting by Name, Description, Address
            Func<IQueryable<Office>, IOrderedQueryable<Office>> orderBy = null;
            if (order != OrderType.None)
            {
                orderBy = sortField switch
                {
                    "Description" => order == OrderType.Ascending ? q => q.OrderBy(s => s.Description) : orderBy = q => q.OrderByDescending(s => s.Description),
                    "Address" => order == OrderType.Ascending ? q => q.OrderBy(s => s.Address) : orderBy = q => q.OrderByDescending(s => s.Address),
                    _ => order == OrderType.Ascending ? q => q.OrderBy(s => s.Name) : orderBy = q => q.OrderByDescending(s => s.Name),
                };
            }
            // adding navigation properties
            Expression<Func<Office, object>> includeCountry = o => o.Country;
            Expression<Func<Office, object>> includeVacancies = o => o.Vacancies;
            Expression<Func<Office, object>>[] navigationProperties =
                new Expression<Func<Office, object>>[] { includeCountry, includeVacancies };

            return await Search(limit: limit, page: page, order: order, orderBy: orderBy, navigationProperties: navigationProperties);
        }

        public async Task<List<OfficeNameIdDto>> GetOfficeIdNamesAsync()
        {
            var officesNameIds = await repositoryOfficeNameId.GetAsync(limit: 0, page: 1);
            var officesNameIdDtos = (List<OfficeNameIdDto>)Mapper.Map<IEnumerable<OfficeNameIdDto>>(officesNameIds.Items);

            return officesNameIdDtos.OrderBy(o => o.Name).ToList();
        }

        public override async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await Repository.IsExistAsync("EXEC @returnVal=sp_checkOfficeById @id, @returnVal", parameters);
        }
    }
}
