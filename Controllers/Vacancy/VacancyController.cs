using CoreWebApi.Library.Enums;
using CoreWebApi.Library.ResponseError;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]"), ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class VacancyController : ControllerBase
    {
        private readonly IVacancyService vacancyService;
        private IResponseError responseBadRequestError;
        private IResponseError responseNotFoundError;

        public VacancyController(IVacancyService vacancyService)
        {
            this.vacancyService = vacancyService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("");
        }

        /// <summary>
        /// Gets a list of VacancyDto's with pagination params and values for search and sorting.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="search">part of Title for searching</param>
        /// <param name="sort_field">Field name for sorting</param>
        /// <param name="order">sort direction: 0 - Ascending or 1 - Descending</param>
        /// <returns>Status 200 and list of VacancyDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Vacancy/get/?limit=10;page=1;search=;sort_field=Idorder=0
        ///     
        /// </remarks>
        /// <response code="200">list of VacancyDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, string search, string sort_field, OrderType order) =>
            Ok(await vacancyService.GetVacanciesSearchResultAsync(limit, page, search, sort_field, order));

        /// <summary>
        /// Gets a list of VacancyDto's for public pages.
        /// </summary>
        /// <param name="page">Requested page</param>
        /// <returns>Status 200 and list of VacancyDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Vacancy/getpublic/?limit=10;page=1;search=;sort_field=Idorder=0
        ///     
        /// </remarks>
        /// <response code="200">list of VacancyDto's</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicAsync(int page) =>
            Ok(await vacancyService.GetVacanciesSearchResultAsync(limit: 10, page, search: "", sort_field: "Id", order: OrderType.Descending));

        /// <summary>
        /// Gets a specific VacancyDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and VacancyDto</returns>
        /// <response code="200">Returns the requested VacancyDto item</response>
        /// <response code="404">If the vacancy with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById([FromRoute] int id)
        {
            var vacancyDto = vacancyService.GetVacancyById(id);
            if (vacancyDto == null)
            {
                responseNotFoundError.Title = "Vacancy Not Found.";
                return NotFound(responseNotFoundError);
            }

            return Ok(vacancyDto);
        }

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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] VacancyDto vacancyDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong vacancy-data.";
                return BadRequest(responseBadRequestError);
            }
            return Created("/api/vacancy/create", vacancyService.CreateVacancy(vacancyDto));
        }

        /// <summary>
        /// Updates an existing Vacancy item.
        /// </summary>
        /// <returns>Status 200 and updated VacancyDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/Vacancy/Update
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update([FromBody] VacancyDto vacancyDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong vacancy-data.";
                return BadRequest(responseBadRequestError);
            }
            if (vacancyService.GetVacancyById(vacancyDto.Id) == null)
            {
                responseNotFoundError.Title = "Vacancy Not Found.";
                return NotFound(responseNotFoundError);
            }

            return Ok(vacancyService.UpdateVacancy(vacancyDto));
        }

        /// <summary>
        /// Deletes a Vacancy Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted VacancyDto object</returns>
        /// <response code="200">Returns the deleted VacancyDto item</response>
        /// <response code="404">If the vacancy with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int id)
        {
            var vacancyToDelete = vacancyService.GetVacancyById(id);
            if (vacancyToDelete == null)
            {
                responseNotFoundError.Title = "Vacancy Not Found.";
                return NotFound(responseNotFoundError);
            }
            vacancyService.DeleteVacancy(id);

            return Ok(vacancyToDelete);
        }
    }
}
