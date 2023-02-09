using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class VacancyService : IVacancyService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Vacancy> repository;
        private readonly IRepository<VacancyTitleId> repositoryVacancyTitleId;

        public VacancyService(
            IMapper mapper,
            IRepository<Vacancy> repository,
            IRepository<VacancyTitleId> repositoryVacancyTitleId)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.repositoryVacancyTitleId = repositoryVacancyTitleId;
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

        public List<VacancyTitleIdDto> GetVacancyTitleIdDto(string title)
        {
            List<SqlParameter> parameters = new List<SqlParameter> { new SqlParameter { ParameterName = "@title", Value = title } };
            var res = repositoryVacancyTitleId.GetByStoredProcedure("EXEC dbo.sp_getTitleIdVacancies @title", parameters.ToArray());

            return mapper.Map<IEnumerable<VacancyTitleIdDto>>(res).ToList();
        }

        public VacancyDto GetVacancyById(int id) => mapper.Map<VacancyDto>(repository.Get(id));

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

        public VacancyDto DeleteVacancy(int id) => mapper.Map<VacancyDto>(repository.Delete(id));

    }
}
