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
    public class CountryController : ControllerBase
    {
        private readonly ICountryService countryService;
        private IResponseError responseBadRequestError;
        private IResponseError responseNotFoundError;

        public CountryController(ICountryService countryService)
        {
            this.countryService = countryService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("");
        }

        /// <summary>
        /// Gets a list of CountryDto's with values for pagination (page number, limit) and sorting by Name.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="order" default="asc">sort direction: 0 - Ascending or 1 - Descending</param>
        /// 
        /// <returns>Status 200 and list of CountryDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Country/get?limit=3;page=1;order=0
        ///     
        /// </remarks>
        /// <response code="200">List of CountryDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, OrderType order ) =>
            Ok(await countryService.GetCountriesSearchResultAsync(limit, page, order));

        /// <summary>
        /// Gets a list of CountryDto's for public pages.
        /// </summary>
        /// <param name="page">Requested page</param>
        /// 
        /// <returns>Status 200 and list of CountryDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Country/get?limit=3;page=1;order=0
        ///     
        /// </remarks>
        /// <response code="200">List of CountryDto's</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicAsync(int page) =>
            Ok(await countryService.GetCountriesSearchResultAsync(limit: 3, page, order: OrderType.Ascending));

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
            if (companySeviceDto == null)
            {
                responseNotFoundError.Title = "Country Not Found.";
                return NotFound(responseNotFoundError);
            }

            return Ok(companySeviceDto);
        }

        /// <summary>
        /// Creates a new Country item.
        /// </summary>
        /// <returns>Status 201 and created CountryDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/country/create
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
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong country-data.";
                return BadRequest(responseBadRequestError);
            }
            return Created("/api/country/create", countryService.CreateCountry(CountryDto));
        }

        /// <summary>
        /// Updates an existing Country item.
        /// </summary>
        /// <returns>Status 200 and updated CountryDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/country/update
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
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong country-data.";
                return BadRequest(responseBadRequestError);
            }
            if (countryService.GetCountryById(CountryDto.Id) == null)
            {
                responseNotFoundError.Title = "Country Not Found.";
                return NotFound(responseNotFoundError);
            }

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
            if (CountryToDelete == null)
            {
                responseNotFoundError.Title = "Country Not Found.";
                return NotFound(responseNotFoundError);
            }
            countryService.DeleteCountry(id);

            return Ok(CountryToDelete);
        }
    }
}
