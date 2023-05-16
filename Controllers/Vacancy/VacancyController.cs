using CoreWebApi.Library;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class VacancyController : AppControllerBase
    {
        private readonly IVacancyService vacancyService;
        private readonly IOfficeService officeService;
        private readonly ICandidateService candidateService;

        public VacancyController(
            IVacancyService vacancyService,
            IOfficeService officeService,
            ICandidateService candidateService)
        {
            this.vacancyService = vacancyService;
            this.officeService = officeService;
            this.candidateService = candidateService;
        }

        /// <summary>
        /// Gets a list of VacancyDto's with pagination params and values for search and sorting.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="search">part of Title for searching</param>
        /// <param name="vacancyStatus">Filter for isActive property: 0 - Active, 1 - Disabled, 2 - All</param>
        /// <param name="officeId">Filter for vacancies by OfficeId</param>
        /// <param name="sortField">Field name for sorting, available fields: Title, Previews</param>
        /// <param name="order">sort direction: 0 - Ascending, 1 - Descending or 2 - None</param>
        /// <returns>Status 200 and list of VacancyDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Vacancy/get?limit=10&amp;page=1&amp;search=&amp;vacancyStatus=2&amp;officeId=&amp;sortField=id&amp;order=0
        ///     
        /// </remarks>
        /// <response code="200">list of VacancyDto's</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, string search, VacancyStatus vacancyStatus, int? officeId, string sortField, OrderType order) =>
            Ok(await vacancyService.GetAsync(limit, page, search ?? "", vacancyStatus, officeId ?? 0, sortField, order));

        /// <summary>
        /// Gets a list of favorite (filtered by candidate email) VacancyDto's with pagination params.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="email">Candidate's email for filtering vacancies</param>
        /// <param name="order">Sorts only by Vacancy Title, sort direction: 0 - Ascending, 1 - Descending or 2 - None</param>
        /// <returns>Status 200 and list of VacancyDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Vacancy/getfavorite?limit=3&amp;page=1&amp;email=&amp;order=0
        ///     
        /// </remarks>
        /// <response code="200">list of VacancyDto's</response>
        [HttpGet]
        [Authorize(Roles = "Admin, Registered")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFavoriteAsync(int limit, int page, string email, OrderType order) =>
            Ok(await vacancyService.GetFavoriteVacanciesSearchResultAsync(limit, page, email, order));

        /// <summary>
        /// Gets a specific VacancyDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and VacancyDto</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/vacancy/getbyid/1
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested VacancyDto item</response>
        /// <response code="404">If the vacancy with given id not found</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Registered")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] int id)
        {
            var vacancyDto = await vacancyService.GetAsync(id);
            if (vacancyDto == null) return NotFound(responseNotFoundError);

            return Ok(vacancyDto);
        }

        /// <summary>
        /// Gets the list of vacancies titles by search searchValue.
        /// </summary>
        /// <param name="searchValue">Search parameter</param>
        /// <param name="officeId">Identifier of the office which vacancy belongs. If it's '' in request query - means all offices.</param>
        /// <returns>Status 200 and the list of vacancies titles. If searchValue is empty - returns all titles</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/vacancy/searchvacanciestitles?searchValue=Test&amp;officeId=3
        ///     
        /// </remarks>
        /// <response code="200">List of vacancies titles</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchVacanciesTitlesAsync([FromQuery] string searchValue, int? officeId) => Ok(await vacancyService.SearchVacanciesTitlesAsync(searchValue ?? "", officeId ?? 0));

        /// <summary>
        /// Creates a new Vacancy item.
        /// </summary>
        /// <returns>Status 201 and created VacancyDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/vacancy/create
        ///     {
        ///        "Title": ".Net Developer",
        ///        "Description": "Test description 1",
        ///        "Previews": "0",
        ///        "IsActive": "true",
        ///        "OfficeId": "1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created VacancyDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] VacancyDto vacancyDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            var createdVacancy = await vacancyService.CreateAsync(vacancyDto);
            // Attaching linked office
            createdVacancy.OfficeDto = await officeService.GetAsync(vacancyDto.OfficeId);

            return Created("/api/vacancy/create", createdVacancy);
        }

        /// <summary>
        /// Updates an existing Vacancy item.
        /// </summary>
        /// <returns>Status 200 and updated VacancyDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/Vacancy/update
        ///     {
        ///        "Id": "1",
        ///        "Title": ".Net Developer",
        ///        "Description": "Test description 1",
        ///        "Previews": "0",
        ///        "IsActive": "true",
        ///        "OfficeId": "1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated VacancyDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the Vacancy with given id not found</response>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] VacancyDto vacancyDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            if (await IsExistAsync(vacancyDto.Id) == false) return NotFound(responseNotFoundError);
            await vacancyService.UpdateAsync(vacancyDto);
            // Entity Framework already tracks the value of vacancyDto.Id, so it's impossible to 
            // attach officeDto and Candidates using another request to EF using the same id.
            // It needs to attach linked models using specified services - here candidateService
            if (vacancyDto.Candidates == null) vacancyDto.Candidates = await candidateService.GetCandidatesByVacancyIdAsync(vacancyDto.Id);

            return Ok(vacancyDto);
        }

        /// <summary>
        /// Partly updates an existing Vacancy Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <param name="patchDocument">Json Patch Document as array of operations</param>
        /// <returns>Status 200 and updated VacancyDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/vacancy/partialvacancyupdate/{id}
        ///     [
        ///         {
        ///             "op": "replace",
        ///             "path": "/previews",
        ///             "value": "1"
        ///         }
        ///     ]
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated VacancyDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the vacancy with given id not found</response>
        [HttpPatch("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartialVacancyUpdateAsync(int id, JsonPatchDocument<object> patchDocument)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            try
            {
                return Ok(await vacancyService.PartialUpdateAsync(id, patchDocument));
            }
            catch
            {
                return BadRequest(responseBadRequestError);
            }
        }

        /// <summary>
        /// Deletes a Vacancy Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted VacancyDto object</returns>
        /// <response code="200">Returns the deleted VacancyDto item</response>
        /// <response code="404">If the vacancy with given id not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            await vacancyService.DeleteAsync(id);

            return Ok();
        }

        private async Task<bool> IsExistAsync(int id) => await vacancyService.IsExistAsync(id);
    }
}
