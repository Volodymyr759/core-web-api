using CoreWebApi.Services.CompanyServiceBL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers.CompanyService
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]")]
    public class CompanyServiceController : ControllerBase
    {
        private readonly ICompanyServiceBL companyServiceBL;

        public CompanyServiceController(ICompanyServiceBL companyServiceBL)
        {
            this.companyServiceBL = companyServiceBL;
        }

        /// <summary>
        /// Gets a list of CompanyServiceDto's with values for pagination (page number, limit) and sorting by Title.
        /// </summary>
        /// <param name="page" default="1">requested page</param>
        /// <param name="sort" default="asc">sort direction: asc or desc</param>
        /// <param name="limit" default="10">number of items per page</param>
        /// <returns>Status 200 and list of CompanyServiceDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/CompanyService/GetAll;page=1;sort=asc;limit=3
        ///     
        /// </remarks>
        /// <response code="200">list of CompanyServiceDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll(int page = 1, string sort = "asc", int limit = 10)
        {
            return Ok(companyServiceBL.GetAllCompanyServices(page, sort, limit));
        }

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
            if (companySeviceDto == null) return NotFound();

            return Ok(companySeviceDto);
        }

        /// <summary>
        /// Creates a new CompanyService item.
        /// </summary>
        /// <returns>Status 201 and created CompanyServiceDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/CompanyService/Create
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
            return Created("/CompanyService/Create", companyServiceBL.CreateCompanyService(companyServiceDto));
        }

        /// <summary>
        /// Updates an existing CompanyService item.
        /// </summary>
        /// <returns>Status 200 and updated CompanyServiceDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/CompanyService/Update
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
            if (companyServiceBL.GetCompanyServiceById(companyServiceDto.Id) == null) return NotFound();

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
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the company service with given id not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult SetIsActive([FromRoute] int id, [FromBody] JsonPatchDocument<CompanyServiceDto> patchDocument)
        {
            var companyServiceDto = companyServiceBL.GetCompanyServiceById(id);
            if (companyServiceDto == null) return NotFound();

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
            if (companyServiceToDelete == null) return NotFound();
            companyServiceBL.DeleteCompanyService(id);

            return Ok(companyServiceToDelete);
        }
    }
}
