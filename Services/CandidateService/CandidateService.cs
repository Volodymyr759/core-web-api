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

        public async Task<SearchResult<CandidateDto>> GetCandidatesSearchResultAsync(int limit, int page, string search, string sortField, OrderType order)
        {
            // search by Title
            Expression<Func<Candidate, bool>> searchQuery = null;
            if (!string.IsNullOrEmpty(search)) searchQuery = t => t.FullName.Contains(search);

            // sorting - newest first
            Func<IQueryable<Candidate>, IOrderedQueryable<Candidate>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Id) : orderBy = q => q.OrderByDescending(s => s.Id);

            var candidates = await repository.GetAllAsync(searchQuery, orderBy);

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

        public async Task<CandidateDto> GetCandidateByIdAsync(int id) => mapper.Map<CandidateDto>(await repository.GetAsync(id));


        public async Task<List<CandidateDto>> GetCandidatesByVacancyIdAsync(int id)
        {
            var candidates = mapper.Map<IEnumerable<CandidateDto>>(await repository.GetAllAsync()).ToList();

            return candidates.FindAll(candidate => candidate.VacancyId == id);
        }

        public async Task<CandidateDto> CreateCandidateAsync(CandidateDto candidateDto)
        {
            var candidate = mapper.Map<Candidate>(candidateDto);

            return mapper.Map<CandidateDto>(await repository.CreateAsync(candidate));
        }

        public async Task UpdateCandidateAsync(CandidateDto candidateDto) =>
            await repository.UpdateAsync(mapper.Map<Candidate>(candidateDto));

        public async Task DeleteCandidateAsync(int id) => await repository.DeleteAsync(id);

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
