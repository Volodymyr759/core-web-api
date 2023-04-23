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
    public class CandidateService : ICandidateService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Candidate> repository;

        public CandidateService(
            IMapper mapper,
            IRepository<Candidate> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public async Task<SearchResult<CandidateDto>> GetCandidatesSearchResultAsync(
            int limit, int page,
            string search, CandidateStatus candidateStatus, int? vacancyId,
            string sortField, OrderType order)
        {
            // search by FullName
            Expression<Func<Candidate, bool>> searchQuery = null;
            if (!string.IsNullOrEmpty(search)) searchQuery = t => t.FullName.Contains(search);

            // sorting FullNames
            Func<IQueryable<Candidate>, IOrderedQueryable<Candidate>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.FullName) : orderBy = q => q.OrderByDescending(s => s.FullName);

            var candidates = await repository.GetAllAsync(searchQuery, orderBy);

            if (candidateStatus != CandidateStatus.All)
                candidates = candidateStatus == CandidateStatus.Active ? candidates.Where(c => c.IsDismissed == false) : candidates.Where(c => c.IsDismissed == true);

            if (vacancyId != null) candidates = candidates.Where(c => c.VacancyId == vacancyId);

            return new SearchResult<CandidateDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)candidates.Count() / limit)),
                SearchCriteria = string.Empty,
                TotalItemCount = candidates.Count(),
                ItemList = (List<CandidateDto>)mapper.Map<IEnumerable<CandidateDto>>(candidates.Skip((page - 1) * limit).Take(limit))
            };
        }

        public async Task<CandidateDto> GetByIdAsync(int id) => mapper.Map<CandidateDto>(await repository.GetAsync(id));

        public async Task<List<CandidateDto>> GetCandidatesByVacancyIdAsync(int id)
        {
            var candidates = mapper.Map<IEnumerable<CandidateDto>>(await repository.GetAllAsync()).ToList();

            return candidates.FindAll(candidate => candidate.VacancyId == id);
        }

        public async Task<CandidateDto> CreateAsync(CandidateDto candidateDto) =>
            mapper.Map<CandidateDto>(await repository.CreateAsync(mapper.Map<Candidate>(candidateDto)));

        public async Task UpdateAsync(CandidateDto candidateDto) =>
            await repository.UpdateAsync(mapper.Map<Candidate>(candidateDto));

        public async Task<CandidateDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument)
        {
            var candidate = await repository.GetAsync(id);
            patchDocument.ApplyTo(candidate);

            return mapper.Map<CandidateDto>(await repository.SaveAsync(candidate));
        }

        public async Task DeleteAsync(int id) => await repository.DeleteAsync(id);

        public async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await repository.IsExistAsync("EXEC @returnVal=sp_checkCandidateById @id, @returnVal", parameters);
        }
    }
}
