using System.Collections.Generic;

namespace CoreWebApi.Services.VacancyService
{
    public interface IVacancyService
    {
        IEnumerable<VacancyDto> GetAllVacancies(int limit, int page, string search, string sort_field, string sort);
        VacancyDto GetVacancyById(int id);
        VacancyDto CreateVacancy(VacancyDto vacancyDto);
        VacancyDto UpdateVacancy(VacancyDto vacancyDto);
        VacancyDto DeleteVacancy(int id);
    }
}
