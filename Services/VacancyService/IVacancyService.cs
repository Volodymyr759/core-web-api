using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IVacancyService : IBaseService<VacancyDto>
    {
        Task<SearchResult<VacancyDto>> GetVacanciesSearchResultAsync(int limit, int page, string search, VacancyStatus? vacancyStatus, int? officeId, string sortField, OrderType order);
        Task<SearchResult<VacancyDto>> GetFavoriteVacanciesSearchResultAsync(int limit, int page, string email, OrderType order);
        Task<List<VacancyDto>> GetVacanciesByOfficeIdAsync(int officeId);
        Task<IEnumerable<StringValue>> SearchVacanciesTitlesAsync(string searchValue, int officeId);
    }
}
