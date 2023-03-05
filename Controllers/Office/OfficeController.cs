﻿using CoreWebApi.Library.Enums;
using CoreWebApi.Library.ResponseError;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class OfficeController : ControllerBase
    {
        private readonly IOfficeService officeService;
        private readonly IResponseError responseBadRequestError;
        private readonly IResponseError responseNotFoundError;

        public OfficeController(IOfficeService officeService)
        {
            this.officeService = officeService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("Wrong office data.");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("Office Not Found.");
        }

        /// <summary>
        /// Gets a list of OfficeDto's with values for pagination (page number, limit) and sorting by Title.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="order">sort direction: 0 - Ascending or 1 - Descending, 2 - None</param>
        /// 
        /// <returns>Status 200 and list of OfficeDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/office/get?limit=3&amp;page=1&amp;order=0
        ///     
        /// </remarks>
        /// <response code="200">list of OfficeDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, OrderType order) =>
            Ok(await officeService.GetOfficesSearchResultAsync(limit, page, order));

        /// <summary>
        /// Gets a list of OfficeDto's for public pages.
        /// </summary>
        /// <param name="page">Requested page</param>
        /// 
        /// <returns>Status 200 and list of OfficeDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/office/getpublic?page=1
        ///     
        /// </remarks>
        /// <response code="200">List of OfficeDto's</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicAsync(int page) =>
            Ok(await officeService.GetOfficesSearchResultAsync(limit: 3, page, order: OrderType.Ascending));

        /// <summary>
        /// Gets a list of OfficeNameIdDto's for public pages.
        /// </summary>
        /// <returns>Status 200 and list of OfficeNameIdDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/office/getpublicofficenameids
        ///     
        /// </remarks>
        /// <response code="200">List of OfficeNameIdDto's</response>
        [HttpGet, AllowAnonymous, ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicOfficeNameIdsAsync() => Ok(await officeService.GetOfficeIdNamesAsync());

        /// <summary>
        /// Gets a specific OfficeDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and OfficeDto</returns>
        /// <response code="200">Returns the requested OfficeDto item</response>
        /// <response code="404">If the office with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var officeDto = await officeService.GetOfficeByIdAsync(id);
            if (officeDto == null) return NotFound(responseNotFoundError);

            return Ok(officeDto);
        }

        /// <summary>
        /// Creates a new Office item.
        /// </summary>
        /// <returns>Status 201 and created OfficeDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/office/create
        ///     {
        ///        "Name": "Main office",
        ///        "Description": "Test description 1",
        ///        "Address": "Test address 1",
        ///        "Latitude": "1.111111",
        ///        "Longitude": "1.111111",
        ///        "CountryId": "1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created OfficeDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] OfficeDto officeDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            return Created("/api/office/create", await officeService.CreateOfficeAsync(officeDto));
        }

        /// <summary>
        /// Updates an existing Office item.
        /// </summary>
        /// <returns>Status 200 and updated OfficeDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/Office/Update
        ///     {
        ///        "Id": "1",
        ///        "Name": "Main office",
        ///        "Description": "Test description 1",
        ///        "Address": "Test address 1",
        ///        "Latitude": "1.111111",
        ///        "Longitude": "1.111111",
        ///        "CountryId": "1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated OfficeDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the office with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] OfficeDto officeDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            if (await IsExistAsync(officeDto.Id) == false) return NotFound(responseNotFoundError);
            await officeService.UpdateOfficeAsync(officeDto);

            return Ok(officeDto);
        }

        /// <summary>
        /// Deletes an Office Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted OfficeDto object</returns>
        /// <response code="200">Returns the deleted OfficeDto item</response>
        /// <response code="404">If the office with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            await officeService.DeleteOfficeAsync(id);

            return Ok();
        }

        private async Task<bool> IsExistAsync(int id) => await officeService.IsExistAsync(id);
    }
}
