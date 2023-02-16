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
    public class VacancyService : IVacancyService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Vacancy> repository;
        private readonly IRepository<StringValue> repositoryStringValue;

        public VacancyService(
            IMapper mapper,
            IRepository<Vacancy> repository,
            IRepository<StringValue> repositoryStringValue)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.repositoryStringValue = repositoryStringValue;
        }

        public async Task<SearchResult<VacancyDto>> GetVacanciesSearchResultAsync(int limit, int page, string search, VacancyStatus? vacancyStatus, int? officeId, string sortfield, OrderType order)
        {
            // search by Title
            Expression<Func<Vacancy, bool>> searchQuery = null;
            if (!string.IsNullOrEmpty(search)) searchQuery = t => t.Title.Contains(search);

            // sorting - newest first
            Func<IQueryable<Vacancy>, IOrderedQueryable<Vacancy>> orderBy = null;
            orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Id) : orderBy = q => q.OrderByDescending(s => s.Id);

            var vacancies = await repository.GetAllAsync(searchQuery, orderBy);
            if (vacancyStatus != null)
            {
                if (vacancyStatus == VacancyStatus.Active) vacancies = vacancies.Where(v => v.IsActive == true);
                else vacancies = vacancies.Where(v => v.IsActive == false);
            }
            if (officeId != 0) vacancies = vacancies.Where(v => v.OfficeId == officeId);

            return new SearchResult<VacancyDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)vacancies.Count() / limit)),
                SearchCriteria = search ?? string.Empty,
                TotalItemCount = vacancies.Count(),
                ItemList = (List<VacancyDto>)mapper.Map<IEnumerable<VacancyDto>>(vacancies.Skip((page - 1) * limit).Take(limit))
            };
        }

        public VacancyDto GetVacancyById(int id) => mapper.Map<VacancyDto>(repository.Get(id));

        public async Task<IEnumerable<StringValue>> SearchVacanciesTitlesAsync(string searchValue)
        {
            var vacanciesTitles = searchValue == null ?
                repositoryStringValue.GetAsync("EXEC dbo.sp_getVacanciesTitles", null) :
                repositoryStringValue.GetAsync("EXEC dbo.sp_getVacanciesTitlesByParam @search",
                    new SqlParameter[] { new SqlParameter { ParameterName = "@search", Value = searchValue } });

            return await vacanciesTitles;
        }

        public VacancyDto CreateVacancy(VacancyDto vacancyDto)
        {
            var vacancy = mapper.Map<Vacancy>(vacancyDto);

            return mapper.Map<VacancyDto>(repository.Create(vacancy));
        }

        public VacancyDto UpdateVacancy(VacancyDto vacancyDto)
        {
            var vacancy = mapper.Map<Vacancy>(vacancyDto);

            return mapper.Map<VacancyDto>(repository.Update(vacancy));
        }

        public async Task DeleteVacancyAsync(int id) => await repository.DeleteAsync(id);

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
