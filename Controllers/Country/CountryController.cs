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
        private readonly IResponseError responseBadRequestError;
        private readonly IResponseError responseNotFoundError;

        public CountryController(ICountryService countryService)
        {
            this.countryService = countryService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("Wrong country data.");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("Country Not Found.");
        }

        /// <summary>
        /// Gets a list of CountryDto's with values for pagination (page number, limit) and sorting by Name.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="order" default="asc">sort direction: 0 - Ascending or 1 - Descending, 2 - None</param>
        /// 
        /// <returns>Status 200 and list of CountryDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Country/get?limit=3&amp;page=1&amp;order=0
        ///     
        /// </remarks>
        /// <response code="200">List of CountryDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, OrderType order) =>
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
        ///     GET /api/Country/get?limit=3&amp;page=1&amp;order=0
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
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var companySeviceDto = await countryService.GetCountryByIdAsync(id);
            if (companySeviceDto == null) return NotFound(responseNotFoundError);

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
        public async Task<IActionResult> CreateAsync([FromBody] CountryDto countryDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);

            return Created("/api/country/create", await countryService.CreateCountryAsync(countryDto));
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
        public async Task<IActionResult> UpdateAsync([FromBody] CountryDto countryDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            if (await IsExistAsync(countryDto.Id) == false) return NotFound(responseNotFoundError);
            await countryService.UpdateCountryAsync(countryDto);

            return Ok(countryDto);
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
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            await countryService.DeleteCountryAsync(id);

            return Ok();
        }

        private async Task<bool> IsExistAsync(int id) => await countryService.IsExistAsync(id);
    }
}
