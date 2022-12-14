using CoreWebApi.Services.TenantService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService tenantService;

        public TenantController(ITenantService tenantService)
        {
            this.tenantService = tenantService;
        }

        /// <summary>
        /// Gets a list of TenantDto's with pagination params and values for search and sorting.
        /// </summary>
        /// <param name="limit" default="10">number of items per page</param>
        /// <param name="page" default="1">requested page</param>
        /// <param name="search">part of first or last name for searching</param>
        /// <param name="sort_field" default="id">field name for sorting</param>
        /// <param name="sort" default="desc">sort direction: asc or desc</param>
        /// <returns>Status 200 and list of TenantDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/tenant/GetAllTenants/?limit=10&amp;page=1&amp;search=j&amp;sort_field=Id&amp;sort=desc
        ///     
        /// </remarks>
        /// <response code="200">list of TenantDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllTenants(int limit = 10, int page = 1, string search = "", string sort_field = "Id", string sort = "desc")
        {
            return Ok(tenantService.GetAllTenants(limit, page, search, sort_field, sort));
        }

        /// <summary>
        /// Gets a specific TenantDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>TenantDto</returns>
        /// <response code="200">Returns the requested TenantDto item</response>
        /// <response code="404">If the tenant with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var tenantDto = tenantService.GetTenantById(id);
            if (tenantDto == null) return NotFound();

            return Ok(tenantDto);
        }

        /// <summary>
        /// Creates a new Tenant item.
        /// </summary>
        /// <returns>Status 201 and created TenantDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/tenant/create
        ///     {
        ///        "FirstName": "First Name",
        ///        "LastName": "Last Name",
        ///        "Email": "test@gmail.com",
        ///        "Phone": "+123123123"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created TenantDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="401">If the user is not logged in</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create(CreateTenantDto createTenantDto)
        {
            return Created("/api/tenant", tenantService.CreateTenant(createTenantDto));
        }

        /// <summary>
        /// Updates an existing Tenant item.
        /// </summary>
        /// <returns>Status 200 and updated TenantDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/tenant/update
        ///     {
        ///        "Id": "4",
        ///        "FirstName": "New First Name",
        ///        "LastName": "Last Name",
        ///        "Email": "test@gmail.com",
        ///        "Phone": "+123123123"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated TenantDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the tenant with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(TenantDto tenantDto)
        {
            if (tenantService.GetTenantById(tenantDto.Id) == null) return NotFound();

            return Ok(tenantService.UpdateTenant(tenantDto));
        }

        /// <summary>
        /// Deletes a TenantDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted TenantDto object</returns>
        /// <response code="200">Returns the deleted TenantDto item</response>
        /// <response code="404">If the tenant with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var tenantDto = tenantService.GetTenantById(id);
            if (tenantDto == null) return NotFound();
            tenantService.DeleteTenant(tenantDto);

            return Ok(tenantDto);
        }

        /// <summary>
        /// Partly updates an existing Tenant Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <param name="patchDocument">Json Patch Document</param>
        /// <returns>Status 200 and updated TenantDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/tenant/PartialTenantUpdate/{id}
        ///     {
        ///        "op": "replace",
        ///        "path": "/email",
        ///        "value": "new_test@gmail.com"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated TenantDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the tenant with given id not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult PartialTenantUpdate(int id, JsonPatchDocument<TenantDto> patchDocument)
        {
            var tenantDto = tenantService.GetTenantById(id);
            if (tenantDto == null) return NotFound();

            patchDocument.ApplyTo(tenantDto, ModelState);

            if (!TryValidateModel(tenantDto)) return ValidationProblem(ModelState);

            return Ok(tenantService.UpdateTenant(tenantDto));
        }
    }
}
