using CoreWebApi.Library.Enums;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class CompanyServiceController : AppControllerBase
    {
        private readonly ICompanyServiceBL companyServiceBL;

        public CompanyServiceController(ICompanyServiceBL companyServiceBL) =>
            this.companyServiceBL = companyServiceBL;

        /// <summary>
        /// Gets a list of CompanyServiceDto's with values for pagination (page number, limit) and sorting by Title.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="companyServiceStatus">Filter for isActive property: 0 - Active, 1 - Disabled, 2 - All</param>
        /// <param name="order">Can sort CompanyServices by Title. Sort direction: 0 - Ascending or 1 - Descending, 2 - None</param>
        /// <returns>Status 200 and list of CompanyServiceDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/companyservice/get?limit=3&amp;page=1&amp;companyServiceStatus=0&amp;order=0
        ///     
        /// </remarks>
        /// <response code="200">list of CompanyServiceDto's</response>
        [HttpGet, AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, CompanyServiceStatus companyServiceStatus, OrderType order) =>
            Ok(await companyServiceBL.GetCompanyServicesSearchResultAsync(limit, page, companyServiceStatus, order));

        /// <summary>
        /// Gets a specific CompanySeviceDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and CompanySeviceDto</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/companyservice/getbyid/1
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested CompanySeviceDto item</response>
        /// <response code="404">If the company service with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var companySeviceDto = await companyServiceBL.GetByIdAsync(id);
            if (companySeviceDto == null) return NotFound(responseNotFoundError);

            return Ok(companySeviceDto);
        }

        /// <summary>
        /// Creates a new CompanyService item.
        /// </summary>
        /// <returns>Status 201 and created CompanyServiceDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/companyservice/create
        ///     {
        ///        "Title": "Lorem Ipsum",
        ///        "Description": "Voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi",
        ///        "ImageUrl": "https://somewhere.com/1",
        ///        "IsActive": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created CompanyServiceDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CompanyServiceDto companyServiceDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            return Created("api/companyservice/create", await companyServiceBL.CreateAsync(companyServiceDto));
        }

        /// <summary>
        /// Updates an existing CompanyService item.
        /// </summary>
        /// <returns>Status 200 and updated CompanyServiceDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/companyservice/update
        ///     {
        ///        "Id": "1",
        ///        "Title": "Lorem Ipsum",
        ///        "Description": "Voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi",
        ///        "ImageUrl": "https://somewhere.com/1",
        ///        "IsActive": 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated CompanyServiceDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        /// <response code="404">If the company service with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] CompanyServiceDto companyServiceDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            if (await IsExistAsync(companyServiceDto.Id) == false) return NotFound(responseNotFoundError);
            await companyServiceBL.UpdateAsync(companyServiceDto);

            return Ok(companyServiceDto);
        }

        /// <summary>
        /// Partly updates an existing CompanyService Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <param name="patchDocument">Json Patch Document</param>
        /// <returns>Status 200 and updated CompanyServiceDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/companyservice/partialserviceupdate/{id}
        ///     {
        ///        "op": "replace",
        ///        "path": "/isactive",
        ///        "value": "false"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated CompanyServiceDto item</response>
        /// <response code="403">If the user hasn't need role</response>
        /// <response code="404">If the company service with given id not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartialServiceUpdateAsync([FromRoute] int id, [FromBody] JsonPatchDocument<object> patchDocument)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            try
            {
                return Ok(await companyServiceBL.PartialUpdateAsync(id, patchDocument));
            }
            catch
            {
                return BadRequest(responseBadRequestError);
            }
        }

        /// <summary>
        /// Deletes a CompanyService Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted CompanyServiceDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/companyservice/delete/1
        ///     
        /// </remarks>
        /// <response code="200">Returns the deleted CompanyServiceDto item</response>
        /// <response code="403">If the user hasn't need role</response>
        /// <response code="404">If the company service with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            await companyServiceBL.DeleteAsync(id);

            return Ok();
        }

        private async Task<bool> IsExistAsync(int id) => await companyServiceBL.IsExistAsync(id);
    }
}
