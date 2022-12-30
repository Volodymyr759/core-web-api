using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.VacancyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreWebApi.Services.CandidateService
{
    public class CandidateService : ICandidateService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Candidate> candidateRepository;
        private readonly IRepository<Vacancy> vacancyRepository;

        public CandidateService(
            IMapper mapper,
            IRepository<Candidate> candidateRepository,
            IRepository<Vacancy> vacancyRepository
            )
        {
            this.mapper = mapper;
            this.candidateRepository = candidateRepository;
            this.vacancyRepository = vacancyRepository;
        }

        public IEnumerable<CandidateDto> GetAllCandidates(int limit, int page, string search, string sort_field, string sort)
        {
            // search by FullName
            Expression<Func<Candidate, bool>> searchQuery = null;
            if (search.Trim().Length > 0) searchQuery = t => t.FullName.Contains(search);

            // sorting - newest first
            Func<IQueryable<Candidate>, IOrderedQueryable<Candidate>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Id) : orderBy = q => q.OrderByDescending(s => s.Id);

            return mapper.Map<IEnumerable<CandidateDto>>(candidateRepository.GetAll(limit, page, searchQuery, orderBy));
        }

        public CandidateDto GetCandidateById(int id)
        {
            var candidateDto = mapper.Map<CandidateDto>(candidateRepository.Get(id));
            if (candidateDto != null)
            {
                candidateDto.VacancyDto = mapper.Map<VacancyDto>(vacancyRepository.Get(candidateDto.VacancyId));
            }

            return candidateDto;
        }
        public CandidateDto CreateCandidate(CandidateDto candidateDto)
        {
            var candidate = mapper.Map<Candidate>(candidateDto);

            return mapper.Map<CandidateDto>(candidateRepository.Create(candidate));
        }

        public CandidateDto UpdateCandidate(CandidateDto candidateDto)
        {
            var candidate = mapper.Map<Candidate>(candidateDto);

            return mapper.Map<CandidateDto>(candidateRepository.Update(candidate));
        }

        public CandidateDto DeleteCandidate(int id)
        {
            return mapper.Map<CandidateDto>(candidateRepository.Delete(id));
        }
    }
}
