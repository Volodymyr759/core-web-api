﻿using CoreWebApi.Library.Enums;
using CoreWebApi.Library.ResponseError;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]"), ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CompanyServiceController : ControllerBase
    {
        private readonly ICompanyServiceBL companyServiceBL;
        private readonly IResponseError responseBadRequestError;
        private readonly IResponseError responseNotFoundError;

        public CompanyServiceController(ICompanyServiceBL companyServiceBL)
        {
            this.companyServiceBL = companyServiceBL;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("Wrong company service data.");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("Company Service Not Found.");
        }

        /// <summary>
        /// Gets a list of CompanyServiceDto's with values for pagination (page number, limit) and sorting by Title.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="order">Sort direction: 0 - Ascending or 1 - Descending</param>
        /// <returns>Status 200 and list of CompanyServiceDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/companyservice/get?limit=3;page=1;order=0;
        ///     
        /// </remarks>
        /// <response code="200">list of CompanyServiceDto's</response>
        [HttpGet, AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, OrderType order) =>
            Ok(await companyServiceBL.GetCompanyServicesSearchResultAsync(limit, page, order));

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
            var companySeviceDto = await companyServiceBL.GetCompanyServiceByIdAsync(id);
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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CompanyServiceDto companyServiceDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            return Created("api/companyservice/create", await companyServiceBL.CreateCompanyServiceAsync(companyServiceDto));
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
        /// <response code="404">If the company service with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] CompanyServiceDto companyServiceDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            if (await IsExistAsync(companyServiceDto.Id) == false) return NotFound(responseNotFoundError);
            await companyServiceBL.UpdateCompanyServiceAsync(companyServiceDto);

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
        ///     PATCH /api/CompanyService/SetIsActive/{id}
        ///     {
        ///        "op": "replace",
        ///        "path": "/isactive",
        ///        "value": "false"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated CompanyServiceDto item</response>
        /// <response code="404">If the company service with given id not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetIsActiveAsync([FromRoute] int id, [FromBody] JsonPatchDocument<CompanyServiceDto> patchDocument)
        {
            //var companyServiceDto = await companyServiceBL.GetCompanyServiceByIdAsync(id);
            //if (await companyServiceBL.IsExistAsync(companyServiceDto.Id) == false) return NotFound(responseNotFoundError);
            //patchDocument.ApplyTo(companyServiceDto, ModelState);
            //if (!TryValidateModel(companyServiceDto)) return ValidationProblem(ModelState);

            //return Ok(companyServiceBL.UpdateCompanyServiceAsync(companyServiceDto));
            // todo: realize like Vacancy partial update!

            return BadRequest();
        }

        /// <summary>
        /// Deletes a CompanyService Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted CompanyServiceDto object</returns>
        /// <response code="200">Returns the deleted CompanyServiceDto item</response>
        /// <response code="404">If the company service with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            await companyServiceBL.DeleteCompanyServiceAsync(id);

            return Ok();
        }

        private async Task<bool> IsExistAsync(int id) => await companyServiceBL.IsExistAsync(id);
    }
}
