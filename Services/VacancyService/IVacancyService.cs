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
        Task<SearchResult<VacancyDto>> GetVacanciesSearchResultAsync(int limit, int page, string search, VacancyStatus? vacancyStatus, int? officeId, string sortField, OrderType order);
        Task<VacancyDto> GetVacancyByIdAsync(int id);
        Task<IEnumerable<StringValue>> SearchVacanciesTitlesAsync(string searchValue, int officeId);
        Task<VacancyDto> CreateVacancyAsync(VacancyDto vacancyDto);
        Task UpdateVacancyAsync(VacancyDto vacancyDto);
        Task DeleteVacancyAsync(int id);
        Task<VacancyDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument);
        Task<bool> IsExistAsync(int id);
    }
}
