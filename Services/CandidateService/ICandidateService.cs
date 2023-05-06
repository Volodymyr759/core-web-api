using CoreWebApi.Library;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICandidateService : IBaseService<CandidateDto>
    {
        Task<ISearchResult<CandidateDto>> GetAsync(
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
