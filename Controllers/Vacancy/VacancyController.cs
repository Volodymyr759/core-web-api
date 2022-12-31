using CoreWebApi.Services.VacancyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers.Vacancy
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]")]
    public class VacancyController : ControllerBase
    {
        private readonly IVacancyService vacancyService;

        public VacancyController(IVacancyService vacancyService)
        {
            this.vacancyService = vacancyService;
        }

        /// <summary>
        /// Gets a list of VacancyDto's with pagination params and values for search and sorting.
        /// </summary>
        /// <param name="limit" default="10">number of items per page</param>
        /// <param name="page" default="1">requested page</param>
        /// <param name="search">part of Title for searching</param>
        /// <param name="sort_field" default="id">field name for sorting</param>
        /// <param name="sort" default="desc">sort direction: asc or desc</param>
        /// <returns>Status 200 and list of VacancyDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Vacancy/GetAll/?limit=10&amp;page=1&amp;search=j&amp;sort_field=Id&amp;sort=desc
        ///     
        /// </remarks>
        /// <response code="200">list of VacancyDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll(int limit = 10, int page = 1, string search = "", string sort_field = "Id", string sort = "desc")
        {
            return Ok(vacancyService.GetAllVacancies(limit, page, search, sort_field, sort));
        }

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
            if (vacancyDto == null) return NotFound();

            return Ok(vacancyDto);
        }

        /// <summary>
        /// Creates a new Vacancy item.
        /// </summary>
        /// <returns>Status 201 and created VacancyDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Vacancy/Create
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
            return Created("/Vacancy/Create", vacancyService.CreateVacancy(vacancyDto));
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
            if (vacancyService.GetVacancyById(vacancyDto.Id) == null) return NotFound();

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
            if (vacancyToDelete == null) return NotFound();
            vacancyService.DeleteVacancy(id);

            return Ok(vacancyToDelete);
        }
    }
}
