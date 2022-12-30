using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.CandidateService;
using CoreWebApi.Services.OfficeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreWebApi.Services.VacancyService
{
    public class VacancyService : IVacancyService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Vacancy> vacancyRepository;
        private readonly IRepository<Office> officeRepository;
        private readonly IRepository<Candidate> candidateRepository;

        public VacancyService(
            IMapper mapper,
            IRepository<Vacancy> vacancyRepository,
            IRepository<Office> officeRepository,
            IRepository<Candidate> candidateRepository)
        {
            this.mapper = mapper;
            this.vacancyRepository = vacancyRepository;
            this.officeRepository = officeRepository;
            this.candidateRepository = candidateRepository;
        }

        public IEnumerable<VacancyDto> GetAllVacancies(int limit, int page, string search, string sort_field, string sort)
        {
            // search by Title
            Expression<Func<Vacancy, bool>> searchQuery = null;
            if (search.Trim().Length > 0) searchQuery = t => t.Title.Contains(search);

            // sorting - newest first
            Func<IQueryable<Vacancy>, IOrderedQueryable<Vacancy>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Id) : orderBy = q => q.OrderByDescending(s => s.Id);

            return mapper.Map<IEnumerable<VacancyDto>>(vacancyRepository.GetAll(limit, page, searchQuery, orderBy));
        }

        public VacancyDto GetVacancyById(int id)
        {
            var vacancyDto = mapper.Map<VacancyDto>(vacancyRepository.Get(id));
            if (vacancyDto != null)
            {
                vacancyDto.CandidateDtos = mapper.Map<IEnumerable<CandidateDto>>(candidateRepository.GetAll().Where(c => c.VacancyId == vacancyDto.Id));
                vacancyDto.OfficeDto = mapper.Map<OfficeDto>(officeRepository.Get(vacancyDto.OfficeId));
            }

            return vacancyDto;
        }

        public VacancyDto CreateVacancy(VacancyDto vacancyDto)
        {
            var vacancy = mapper.Map<Vacancy>(vacancyDto);

            return mapper.Map<VacancyDto>(vacancyRepository.Create(vacancy));
        }

        public VacancyDto UpdateVacancy(VacancyDto vacancyDto)
        {
            var vacancy = mapper.Map<Vacancy>(vacancyDto);

            return mapper.Map<VacancyDto>(vacancyRepository.Update(vacancy));
        }

        public VacancyDto DeleteVacancy(int id)
        {
            return mapper.Map<VacancyDto>(vacancyRepository.Delete(id));
        }
    }
}
