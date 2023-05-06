using CoreWebApi.Library;
using CoreWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IVacancyService : IBaseService<VacancyDto>
    {
        Task<ISearchResult<VacancyDto>> GetAsync(int limit, int page, string search, VacancyStatus? vacancyStatus, int? officeId, string sortField, OrderType order);

        Task<ISearchResult<VacancyDto>> GetFavoriteVacanciesSearchResultAsync(int limit, int page, string email, OrderType order);

        Task<IEnumerable<VacancyDto>> GetVacanciesByOfficeIdAsync(int officeId);

        Task<IEnumerable<StringValue>> SearchVacanciesTitlesAsync(string searchValue, int officeId);
    }
}
