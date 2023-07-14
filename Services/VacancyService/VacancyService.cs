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
    public class VacancyService : AppBaseService<Vacancy, VacancyDto>, IVacancyService
    {
        private readonly IRepository<StringValue> repositoryStringValue;
        private readonly IRepository<Office> repositoryOffice;
        private readonly IRepository<Candidate> repositoryCandidate;

        public VacancyService(
            IMapper mapper,
            IRepository<Vacancy> repository,
            IRepository<StringValue> repositoryStringValue,
            IRepository<Office> repositoryOffice,
            IRepository<Candidate> repositoryCandidate,
            ISearchResult<VacancyDto> searchResult,
            IServiceResult<Vacancy> serviceResult) : base(mapper, repository, searchResult, serviceResult)
        {

            this.repositoryStringValue = repositoryStringValue;
            this.repositoryOffice = repositoryOffice;
            this.repositoryCandidate = repositoryCandidate;
        }

        public async Task<ISearchResult<VacancyDto>> GetAsync(int limit, int page, string search, VacancyStatus? vacancyStatus, int? officeId, string sortField, OrderType order)
        {
            // filtering
            var filters = new List<Expression<Func<Vacancy, bool>>>();
            if (!string.IsNullOrEmpty(search)) filters.Add(t => t.Title.Contains(search));
            if (vacancyStatus == VacancyStatus.Active) filters.Add(v => v.IsActive == true);
            if (vacancyStatus == VacancyStatus.Disabled) filters.Add(v => v.IsActive == false);
            if (officeId != 0) filters.Add(v => v.OfficeId == officeId);

            // sorting by Title or Previews
            Func<IQueryable<Vacancy>, IOrderedQueryable<Vacancy>> orderBy = null;
            if (order != OrderType.None)
            {
                orderBy = sortField switch
                {
                    "Previews" => order == OrderType.Ascending ? q => q.OrderBy(v => v.Previews) : orderBy = q => q.OrderByDescending(v => v.Previews),
                    _ => order == OrderType.Ascending ? q => q.OrderBy(v => v.Title) : orderBy = q => q.OrderByDescending(v => v.Title),
                };
            }
            // adding navigation properties
            Expression<Func<Vacancy, object>> includeOffice = v => v.Office;
            Expression<Func<Vacancy, object>> includeCandidates = v => v.Candidates;
            Expression<Func<Vacancy, object>>[] navigationProperties =
                new Expression<Func<Vacancy, object>>[] { includeOffice, includeCandidates };

            return await Search(limit: limit, page: page, search: search, filters: filters, order: order, orderBy: orderBy, navigationProperties: navigationProperties);
        }

        public async Task<ISearchResult<VacancyDto>> GetFavoriteVacanciesSearchResultAsync(int limit, int page, string email, OrderType order)
        {
            var favoriteVacancies = await Repository.GetAsync("EXEC dbo.[sp_getVacanciesByCandidateEmail] @email",
                    new SqlParameter[] { new SqlParameter("@email", email) });
            var paginatedFavoriteVacancies = favoriteVacancies.Skip((page - 1) * limit).Take(limit);

            // Attaching offices and candidates
            var officesData = await repositoryOffice.GetAsync(limit: 0, page: 1);
            var candidatesData = await repositoryCandidate.GetAsync(limit: 0, page: 1);
            foreach (var vacancy in paginatedFavoriteVacancies)
            {
                vacancy.Office = officesData.Items.Where(o => o.Id == vacancy.OfficeId).FirstOrDefault();
                vacancy.Candidates = candidatesData.Items.Where(c => c.VacancyId == vacancy.Id).ToList();
            }

            return new SearchResult<VacancyDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)favoriteVacancies.Count() / limit)),
                SearchCriteria = email ?? string.Empty,
                TotalItemCount = favoriteVacancies.Count(),
                ItemList = (List<VacancyDto>)Mapper.Map<IEnumerable<VacancyDto>>(paginatedFavoriteVacancies)
            };
        }

        new public async Task<VacancyDto> GetAsync(int id)
        {
            Expression<Func<Vacancy, bool>> query = v => v.Id == id;
            Expression<Func<Vacancy, object>>[] navigationProperties =
                new Expression<Func<Vacancy, object>>[] { v => v.Office, v => v.Candidates };
            var vacancy = await Repository.GetAsync(query, navigationProperties);

            return Mapper.Map<VacancyDto>(vacancy);
        }

        public async Task<IEnumerable<StringValue>> SearchVacanciesTitlesAsync(string searchValue, int officeId)
        {
            List<StringValue> vacanciesTitles = officeId == 0 ?
                (await repositoryStringValue.GetAsync("EXEC dbo.sp_getVacanciesTitles", null)).ToList() :
                vacanciesTitles = (await repositoryStringValue.GetAsync("EXEC dbo.[sp_getVacanciesTitlesByOfficeId] @id",
                    new SqlParameter[] { new SqlParameter { ParameterName = "@id", Value = officeId } })).ToList();
            vacanciesTitles = vacanciesTitles.FindAll(v => v.Value.ToLower().Contains(searchValue.ToLower()));
            return vacanciesTitles;
        }

        public async Task<IEnumerable<VacancyDto>> GetVacanciesByOfficeIdAsync(int officeId)
        {
            var filters = new List<Expression<Func<Vacancy, bool>>> { v => v.OfficeId == officeId };
            Func<IQueryable<Vacancy>, IOrderedQueryable<Vacancy>> orderBy = q => q.OrderBy(v => v.Title);
            var data = await Repository.GetAsync(filters: filters, orderBy: orderBy);

            return Mapper.Map<IEnumerable<VacancyDto>>(data.Items).ToList();
        }

        public override async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await Repository.IsExistAsync("EXEC @returnVal=sp_checkVacancyById @id, @returnVal", parameters);
        }
    }
}
