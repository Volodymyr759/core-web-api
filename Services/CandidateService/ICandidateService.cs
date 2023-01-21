using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICandidateService
    {
        Task<SearchResult<CandidateDto>> GetCandidatesSearchResultAsync(int limit, int page, string search, string sort_field, OrderType order);
        CandidateDto GetCandidateById(int id);
        CandidateDto CreateCandidate(CandidateDto candidateDto);
        CandidateDto UpdateCandidate(CandidateDto candidateDto);
        CandidateDto DeleteCandidate(int id);
    }
}
