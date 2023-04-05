using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICandidateService : IBaseService<CandidateDto>
    {
        Task<SearchResult<CandidateDto>> GetCandidatesSearchResultAsync(
            int limit,
            int page,
            string search,
            CandidateStatus candidateStatus,
            int? vacancyId,
            string sortField,
            OrderType order);

        Task<List<CandidateDto>> GetCandidatesByVacancyIdAsync(int id);
    }
}
