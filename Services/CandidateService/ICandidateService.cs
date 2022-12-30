using System.Collections.Generic;

namespace CoreWebApi.Services.CandidateService
{
    public interface ICandidateService
    {
        IEnumerable<CandidateDto> GetAllCandidates(int limit, int page, string search, string sort_field, string sort);
        CandidateDto GetCandidateById(int id);
        CandidateDto CreateCandidate(CandidateDto candidateDto);
        CandidateDto UpdateCandidate(CandidateDto candidateDto);
        CandidateDto DeleteCandidate(int id);
    }
}
