using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IVacancyService
    {
        Task<SearchResult<VacancyDto>> GetVacanciesSearchResultAsync(int limit, int page, string search, VacancyStatus? vacancyStatus, int? officeId, string sortfield, OrderType order);
        VacancyDto GetVacancyById(int id);
        VacancyDto CreateVacancy(VacancyDto vacancyDto);
        VacancyDto UpdateVacancy(VacancyDto vacancyDto);
        VacancyDto DeleteVacancy(int id);
    }
}
