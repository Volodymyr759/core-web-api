using CoreWebApi.Services.OfficeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers.Office
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]")]
    public class OfficeController : ControllerBase
    {
        private readonly IOfficeService officeService;

        public OfficeController(IOfficeService officeService)
        {
            this.officeService = officeService;
        }

        /// <summary>
        /// Gets a list of OfficeDto's with values for pagination (page number, limit) and sorting by Title.
        /// </summary>
        /// <param name="page" default="1">requested page</param>
        /// <param name="sort" default="asc">sort direction: asc or desc</param>
        /// <param name="limit" default="10">number of items per page</param>
        /// <returns>Status 200 and list of OfficeDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Office/GetAll;page=1;sort=asc;limit=3
        ///     
        /// </remarks>
        /// <response code="200">list of OfficeDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll(int page = 1, string sort = "asc", int limit = 10)
        {
            return Ok(officeService.GetAllOffices(page, sort, limit));
        }

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
        public IActionResult GetById([FromRoute] int id)
        {
            var officeDto = officeService.GetOfficeById(id);
            if (officeDto == null) return NotFound();

            return Ok(officeDto);
        }

        /// <summary>
        /// Creates a new Office item.
        /// </summary>
        /// <returns>Status 201 and created OfficeDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Office/Create
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
        public IActionResult Create([FromBody] OfficeDto OfficeDto)
        {
            return Created("/Office/Create", officeService.CreateOffice(OfficeDto));
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
        public IActionResult Update([FromBody] OfficeDto OfficeDto)
        {
            if (officeService.GetOfficeById(OfficeDto.Id) == null) return NotFound();

            return Ok(officeService.UpdateOffice(OfficeDto));
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
        public IActionResult Delete([FromRoute] int id)
        {
            var officeToDelete = officeService.GetOfficeById(id);
            if (officeToDelete == null) return NotFound();
            officeService.DeleteOffice(id);

            return Ok(officeToDelete);
        }
    }
}
