using CoreWebApi.Services.CountryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]")]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService countryService;

        public CountryController(ICountryService countryService)
        {
            this.countryService = countryService;
        }

        /// <summary>
        /// Gets a list of CountryDto's with values for pagination (page number, limit) and sorting by Name.
        /// </summary>
        /// <param name="page" default="1">requested page</param>
        /// <param name="sort" default="asc">sort direction: asc or desc</param>
        /// <param name="limit" default="10">number of items per page</param>
        /// <returns>Status 200 and list of CountryDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Country/GetAll;page=1;sort=asc;limit=3
        ///     
        /// </remarks>
        /// <response code="200">list of CountryDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll(int page = 1, string sort = "asc", int limit = 10)
        {
            return Ok(countryService.GetAllCountries(page, sort, limit));
        }

        /// <summary>
        /// Gets a specific CountryDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and CountryDto</returns>
        /// <response code="200">Returns the requested CountryDto item</response>
        /// <response code="404">If the country with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById([FromRoute] int id)
        {
            var companySeviceDto = countryService.GetCountryById(id);
            if (companySeviceDto == null) return NotFound();

            return Ok(companySeviceDto);
        }

        /// <summary>
        /// Creates a new Country item.
        /// </summary>
        /// <returns>Status 201 and created CountryDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Country/Create
        ///     {
        ///        "Name": "Australia",
        ///        "Code": "AUS"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created CountryDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] CountryDto CountryDto)
        {
            return Created("/Country/Create", countryService.CreateCountry(CountryDto));
        }

        /// <summary>
        /// Updates an existing Country item.
        /// </summary>
        /// <returns>Status 200 and updated CountryDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/Country/Update
        ///     {
        ///        "Id": "1",
        ///        "Name": "Australia",
        ///        "Code": "AUS"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated CountryDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the country with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update([FromBody] CountryDto CountryDto)
        {
            if (countryService.GetCountryById(CountryDto.Id) == null) return NotFound();

            return Ok(countryService.UpdateCountry(CountryDto));
        }

        /// <summary>
        /// Deletes a Country Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted CountryDto object</returns>
        /// <response code="200">Returns the deleted CountryDto item</response>
        /// <response code="404">If the country with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int id)
        {
            var CountryToDelete = countryService.GetCountryById(id);
            if (CountryToDelete == null) return NotFound();
            countryService.DeleteCountry(id);

            return Ok(CountryToDelete);
        }
    }
}
