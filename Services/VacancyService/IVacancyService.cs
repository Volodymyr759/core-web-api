using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IVacancyService
    {
        Task<SearchResult<VacancyDto>> GetVacanciesSearchResultAsync(int limit, int page, string search, VacancyStatus? vacancyStatus, int? officeId, string sortfield, OrderType order);
        VacancyDto GetVacancyById(int id);
        Task<IEnumerable<StringValue>> SearchVacanciesTitlesAsync(string searchValue);
        VacancyDto CreateVacancy(VacancyDto vacancyDto);
        VacancyDto UpdateVacancy(VacancyDto vacancyDto);
        VacancyDto DeleteVacancy(int id);
        Task<VacancyDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument);
        Task<bool> IsExistAsync(int id);
    }
}
