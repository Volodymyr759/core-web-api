using CoreWebApi.Library.ResponseError;
using CoreWebApi.Library.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CoreWebApi.Services;

namespace CoreWebApi.Controllers
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]"), ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;
        private IResponseError responseBadRequestError;
        private IResponseError responseNotFoundError;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("");
        }

        /// <summary>
        /// Gets a list of EmployeeDto's with pagination params and values for search and sorting.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="search">Part of full name for searching</param>
        /// <param name="sortField">Field name for sorting</param>
        /// <param name="order">Sort direction: 0 - Ascending or 1 - Descending</param>
        /// <returns>Status 200 and list of EmployeeDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/employee/get/?limit=10&amp;page=1&amp;search=j&amp;sortField=Id&amp;sort=desc
        ///     
        /// </remarks>
        /// <response code="200">list of EmployeeDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, string search, string sortField, OrderType order) =>
            Ok(await employeeService.GetEmployeesSearchResultAsync(limit, page, search ?? "", sortField ?? "FullName", order));

        /// <summary>
        /// Gets a list of EmployeeDto's for public pages.
        /// </summary>
        /// <param name="page">Requested page</param>
        /// <returns>Status 200 and list of EmployeeDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/employee/getpublic?page=1
        ///     
        /// </remarks>
        /// <response code="200">List of EmployeeDto's</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicAsync(int page) =>
            Ok(await employeeService.GetEmployeesSearchResultAsync(limit: 3, page, search: "", sort_field: "Id", order: OrderType.Ascending));

        /// <summary>
        /// Gets a specific EmployeeDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and EmployeeDto</returns>
        /// <response code="200">Returns the requested EmployeeDto item</response>
        /// <response code="404">If the employee with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById([FromRoute] int id)
        {
            var employeeDto = employeeService.GetEmployeeById(id);
            if (employeeDto == null)
            {
                responseNotFoundError.Title = "Employee Not Found.";
                return NotFound(responseNotFoundError);
            }

            return Ok(employeeDto);
        }

        /// <summary>
        /// Creates a new Employee item.
        /// </summary>
        /// <returns>Status 201 and created EmployeeDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Employee/create
        ///     {
        ///        "FullName": "John Done",
        ///        "Email": "john@gmail.com",
        ///        "Position": "CEO",
        ///        "Description": "CEO description",
        ///        "AvatarUrl": "https://www.somewhere.com/1",
        ///        "OfficeId": "1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created EmployeeDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] EmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong employee-data.";
                return BadRequest(responseBadRequestError);
            }
            return Created("/api/Employee/create", employeeService.CreateEmployee(employeeDto));
        }

        /// <summary>
        /// Updates an existing Employee item.
        /// </summary>
        /// <returns>Status 200 and updated EmployeeDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/employee/update
        ///     {
        ///        "Id": "1",
        ///        "FullName": "John Done",
        ///        "Email": "john@gmail.com",
        ///        "Position": "CEO",
        ///        "Description": "CEO description",
        ///        "AvatarUrl": "https://www.somewhere.com/1",
        ///        "OfficeId": "1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated EmployeeDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the employee with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update([FromBody] EmployeeDto employeeDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong employee-data.";
                return BadRequest(responseBadRequestError);
            }
            if (employeeService.GetEmployeeById(employeeDto.Id) == null)
            {
                responseNotFoundError.Title = "Employee Not Found.";
                return NotFound(responseNotFoundError);
            }

            return Ok(employeeService.UpdateEmployee(employeeDto));
        }

        /// <summary>
        /// Deletes a employee Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted EmployeeDto object</returns>
        /// <response code="200">Returns the deleted EmployeeDto item</response>
        /// <response code="404">If the employee with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int id)
        {
            var employeeToDelete = employeeService.GetEmployeeById(id);
            if (employeeToDelete == null)
            {
                responseNotFoundError.Title = "Employee Not Found.";
                return NotFound(responseNotFoundError);
            }
            employeeService.DeleteEmployee(id);

            return Ok(employeeToDelete);
        }
    }
}
