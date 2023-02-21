using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICandidateService
    {
        Task<SearchResult<CandidateDto>> GetCandidatesSearchResultAsync(int limit, int page, string search, string sortField, OrderType order);
        Task<CandidateDto> GetCandidateByIdAsync(int id);
        Task<CandidateDto> CreateCandidateAsync(CandidateDto candidateDto);
        Task UpdateCandidateAsync(CandidateDto candidateDto);
        Task DeleteCandidateAsync(int id);
        Task<bool> IsExistAsync(int id);
    }
}
