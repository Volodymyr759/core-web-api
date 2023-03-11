using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface ICandidateService
    {
        Task<SearchResult<CandidateDto>> GetCandidatesSearchResultAsync(
            int limit, int page, 
            string search, CandidateStatus candidateStatus, int? vacancyId, 
            string sortField, OrderType order);
        Task<CandidateDto> GetCandidateByIdAsync(int id);
        Task<List<CandidateDto>> GetCandidatesByVacancyIdAsync(int id);
        Task<CandidateDto> CreateCandidateAsync(CandidateDto candidateDto);
        Task UpdateCandidateAsync(CandidateDto candidateDto);
        Task<CandidateDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument);
        Task DeleteCandidateAsync(int id);
        Task<bool> IsExistAsync(int id);
    }
}
