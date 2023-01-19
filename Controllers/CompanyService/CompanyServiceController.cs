using CoreWebApi.Library.Enums;
using CoreWebApi.Library.ResponseError;
using CoreWebApi.Services.CompanyServiceBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers.CompanyService
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]"), ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CompanyServiceController : ControllerBase
    {
        private readonly ICompanyServiceBL companyServiceBL;
        private IResponseError responseBadRequestError;
        private IResponseError responseNotFoundError;

        public CompanyServiceController(ICompanyServiceBL companyServiceBL)
        {
            this.companyServiceBL = companyServiceBL;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("");
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
        ///     GET /api/companyservice/get?limit=3&page=1&order=0;
        ///     
        /// </remarks>
        /// <response code="200">list of CompanyServiceDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, OrderType order) =>
            Ok(await companyServiceBL.GetCompanyServicesSearchResultAsync(limit, page, order));

        /// <summary>
        /// Gets a list of CompanyServiceDto's with values for pagination (page number, limit) and sorting by Title for public pages.
        /// </summary>
        /// <param name="page">Requested page</param>
        /// <returns>Status 200 and list of CompanyServiceDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/companyservice/getpublic?page=1
        ///     
        /// </remarks>
        /// <response code="200">list of CompanyServiceDto's</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicAsync(int page) =>
            Ok(await companyServiceBL.GetCompanyServicesSearchResultAsync(limit: 3, page, order: OrderType.Ascending));

        /// <summary>
        /// Gets a specific CompanySeviceDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and CompanySeviceDto</returns>
        /// <response code="200">Returns the requested CompanySeviceDto item</response>
        /// <response code="404">If the company service with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById([FromRoute] int id)
        {
            var companySeviceDto = companyServiceBL.GetCompanyServiceById(id);
            if (companySeviceDto == null)
            {
                responseNotFoundError.Title = "Company Service Not Found.";
                return NotFound(responseNotFoundError);
            }

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
        public IActionResult Create([FromBody] CompanyServiceDto companyServiceDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong company service - data.";
                return BadRequest(responseBadRequestError);
            }
            return Created("api/companyservice/create", companyServiceBL.CreateCompanyService(companyServiceDto));
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
        public IActionResult Update([FromBody] CompanyServiceDto companyServiceDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong company service - data.";
                return BadRequest(responseBadRequestError);
            }
            if (companyServiceBL.GetCompanyServiceById(companyServiceDto.Id) == null)
            {
                responseNotFoundError.Title = "Company Service Not Found.";
                return NotFound(responseNotFoundError);
            }

            return Ok(companyServiceBL.UpdateCompanyService(companyServiceDto));
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
        public IActionResult SetIsActive([FromRoute] int id, [FromBody] JsonPatchDocument<CompanyServiceDto> patchDocument)
        {
            var companyServiceDto = companyServiceBL.GetCompanyServiceById(id);
            if (companyServiceDto == null)
            {
                responseNotFoundError.Title = "Company Service Not Found.";
                return NotFound(responseNotFoundError);
            }

            patchDocument.ApplyTo(companyServiceDto, ModelState);
            if (!TryValidateModel(companyServiceDto)) return ValidationProblem(ModelState);

            return Ok(companyServiceBL.UpdateCompanyService(companyServiceDto));
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
        public IActionResult Delete([FromRoute] int id)
        {
            var companyServiceToDelete = companyServiceBL.GetCompanyServiceById(id);
            if (companyServiceToDelete == null)
            {
                responseNotFoundError.Title = "Company Service Not Found.";
                return NotFound(responseNotFoundError);
            }
            companyServiceBL.DeleteCompanyService(id);

            return Ok(companyServiceToDelete);
        }
    }
}
