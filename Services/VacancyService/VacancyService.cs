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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class VacancyService : BaseService<Vacancy>, IVacancyService
    {
        private readonly IRepository<StringValue> repositoryStringValue;
        private readonly IRepository<Office> repositoryOffice;
        private readonly IRepository<Candidate> repositoryCandidate;

        public VacancyService(
            IMapper mapper,
            IRepository<Vacancy> repository,
            IRepository<StringValue> repositoryStringValue,
            IRepository<Office> repositoryOffice,
            IRepository<Candidate> repositoryCandidate) : base(mapper, repository)
        {

            this.repositoryStringValue = repositoryStringValue;
            this.repositoryOffice = repositoryOffice;
            this.repositoryCandidate = repositoryCandidate;
        }

        public async Task<SearchResult<VacancyDto>> GetVacanciesSearchResultAsync(int limit, int page, string search, VacancyStatus? vacancyStatus, int? officeId, string sortField, OrderType order)
        {
            // search by Title
            Expression<Func<Vacancy, bool>> searchQuery = null;
            if (!string.IsNullOrEmpty(search)) searchQuery = t => t.Title.Contains(search);

            // sorting by vacancy title
            Func<IQueryable<Vacancy>, IOrderedQueryable<Vacancy>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);

            Expression<Func<Vacancy, object>> include = v => v.Office;
            var vacancies = await repository.GetAllAsync(searchQuery, orderBy, include);

            // Filtering
            if (vacancyStatus == VacancyStatus.Active) vacancies = vacancies.Where(v => v.IsActive == true);
            if (vacancyStatus == VacancyStatus.Disabled) vacancies = vacancies.Where(v => v.IsActive == false);
            if (officeId != 0) vacancies = vacancies.Where(v => v.OfficeId == officeId);

            // Attaching candidates
            var paginatedVacancies = vacancies.Skip((page - 1) * limit).Take(limit);
            var candidates = await repositoryCandidate.GetAllAsync();
            foreach (var vacancy in paginatedVacancies)
                vacancy.Candidates = candidates.Where(c => c.VacancyId == vacancy.Id).ToList();

            return new SearchResult<VacancyDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)vacancies.Count() / limit)),
                SearchCriteria = search ?? string.Empty,
                TotalItemCount = vacancies.Count(),
                ItemList = (List<VacancyDto>)mapper.Map<IEnumerable<VacancyDto>>(paginatedVacancies)
            };
        }

        public async Task<SearchResult<VacancyDto>> GetFavoriteVacanciesSearchResultAsync(int limit, int page, string email, OrderType order)
        {
            var favoriteVacancies = await repository.GetAsync("EXEC dbo.[sp_getVacanciesByCandidateEmail] @email",
                    new SqlParameter[] { new SqlParameter("@email", email) });
            var paginatedFavoriteVacancies = favoriteVacancies.Skip((page - 1) * limit).Take(limit);

            // Attaching offices and candidates
            var offices = await repositoryOffice.GetAllAsync();
            var candidates = await repositoryCandidate.GetAllAsync();
            foreach (var vacancy in paginatedFavoriteVacancies)
            {
                vacancy.Office = offices.Where(o => o.Id == vacancy.OfficeId).FirstOrDefault();
                vacancy.Candidates = candidates.Where(c => c.VacancyId == vacancy.Id).ToList();
            }

            return new SearchResult<VacancyDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)favoriteVacancies.Count() / limit)),
                SearchCriteria = email ?? string.Empty,
                TotalItemCount = favoriteVacancies.Count(),
                ItemList = (List<VacancyDto>)mapper.Map<IEnumerable<VacancyDto>>(paginatedFavoriteVacancies)
            };
        }

        public async Task<VacancyDto> GetByIdAsync(int id)
        {
            Expression<Func<Vacancy, bool>> searchQuery = v => v.Id == id;
            Expression<Func<Vacancy, object>> include = v => v.Office;
            var vacancy = await repository.GetAsync(searchQuery, include);

            // Attaching candidates
            Expression<Func<Candidate, bool>> query = c => c.VacancyId == vacancy.Id;
            vacancy.Candidates = (await repositoryCandidate.GetAllAsync(query, null)).ToList();

            return mapper.Map<VacancyDto>(vacancy);
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

        public async Task<List<VacancyDto>> GetVacanciesByOfficeIdAsync(int officeId)
        {
            var vacancies = mapper.Map<IEnumerable<VacancyDto>>(await repository.GetAllAsync()).ToList();

            return vacancies.FindAll(v => v.OfficeId == officeId);
        }

        public async Task<VacancyDto> CreateAsync(VacancyDto vacancyDto)
        {
            var vacancy = mapper.Map<Vacancy>(vacancyDto);

            return mapper.Map<VacancyDto>(await repository.CreateAsync(vacancy));
        }

        public async Task UpdateAsync(VacancyDto vacancyDto) =>
            await repository.UpdateAsync(mapper.Map<Vacancy>(vacancyDto));

        public async Task DeleteAsync(int id) => await repository.DeleteAsync(id);

        public async Task<VacancyDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument)
        {
            var vacancy = await repository.GetAsync(id);
            patchDocument.ApplyTo(vacancy);
            return mapper.Map<VacancyDto>(await repository.SaveAsync(vacancy));
        }

        public async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await repository.IsExistAsync("EXEC @returnVal=sp_checkVacancyById @id, @returnVal", parameters);
        }

    }
}
