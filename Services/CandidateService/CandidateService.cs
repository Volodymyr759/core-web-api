﻿using AutoMapper;
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
    public class CandidateService : AppBaseService<Candidate, CandidateDto>, ICandidateService
    {
        public CandidateService(
            IMapper mapper,
            IRepository<Candidate> repository,
            ISearchResult<CandidateDto> searchResult,
            IServiceResult<Candidate> serviceResult) : base(mapper, repository, searchResult, serviceResult) { }

        public async Task<ISearchResult<CandidateDto>> GetAsync(
            int limit,
            int page,
            string search,
            CandidateStatus candidateStatus,
            int? vacancyId,
            string sortField,
            OrderType order)
        {
            // filtering
            var filters = new List<Expression<Func<Candidate, bool>>>();
            if (!string.IsNullOrEmpty(search)) filters.Add(t => t.FullName.Contains(search));
            if (candidateStatus == CandidateStatus.Active) filters.Add(c => c.IsDismissed == false);
            if (candidateStatus == CandidateStatus.Dismissed) filters.Add(c => c.IsDismissed == true);
            if (vacancyId != null) filters.Add(c => c.VacancyId == vacancyId);

            // sorting
            Func<IQueryable<Candidate>, IOrderedQueryable<Candidate>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.FullName) : orderBy = q => q.OrderByDescending(s => s.FullName);

            return await Search(limit: limit, page: page, search: search, filters: filters, order: order, orderBy: orderBy);
        }

        public async Task<List<CandidateDto>> GetCandidatesByVacancyIdAsync(int id)
        {
            // filtering
            var filters = new List<Expression<Func<Candidate, bool>>> { c => c.VacancyId == id };

            // sorting
            Func<IQueryable<Candidate>, IOrderedQueryable<Candidate>> orderBy = q => q.OrderBy(s => s.FullName);

            var data = await Repository.GetAsync(filters: filters, orderBy: orderBy);

            return Mapper.Map<IEnumerable<CandidateDto>>(data.Items).ToList();
        }

        public override async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await Repository.IsExistAsync("EXEC @returnVal=sp_checkCandidateById @id, @returnVal", parameters);
        }
    }
}
