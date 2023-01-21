using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
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

        public async Task<SearchResult<CandidateDto>> GetCandidatesSearchResultAsync(int limit, int page, string search, string sort_field, OrderType order)
        {
            // search by Title
            Expression<Func<Candidate, bool>> searchQuery = null;
            if (search.Trim().Length > 0) searchQuery = t => t.FullName.Contains(search);

            // sorting - newest first
            Func<IQueryable<Candidate>, IOrderedQueryable<Candidate>> orderBy = null;
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

        public CandidateDto GetCandidateById(int id) => mapper.Map<CandidateDto>(repository.Get(id));

        public CandidateDto CreateCandidate(CandidateDto candidateDto)
        {
            var candidate = mapper.Map<Candidate>(candidateDto);

            return mapper.Map<CandidateDto>(repository.Create(candidate));
        }

        public CandidateDto UpdateCandidate(CandidateDto candidateDto)
        {
            var candidate = mapper.Map<Candidate>(candidateDto);

            return mapper.Map<CandidateDto>(repository.Update(candidate));
        }

        public CandidateDto DeleteCandidate(int id) => mapper.Map<CandidateDto>(repository.Delete(id));
    }
}
