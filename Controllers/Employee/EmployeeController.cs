using CoreWebApi.Services.EmployeeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers.Employee
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        /// <summary>
        /// Gets a list of EmployeeDto's with pagination params and values for search and sorting.
        /// </summary>
        /// <param name="limit" default="10">number of items per page</param>
        /// <param name="page" default="1">requested page</param>
        /// <param name="search">part of full name for searching</param>
        /// <param name="sort_field" default="id">field name for sorting</param>
        /// <param name="sort" default="desc">sort direction: asc or desc</param>
        /// <returns>Status 200 and list of EmployeeDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Employee/GetAll/?limit=10&amp;page=1&amp;search=j&amp;sort_field=Id&amp;sort=desc
        ///     
        /// </remarks>
        /// <response code="200">list of TenantDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll(int limit = 10, int page = 1, string search = "", string sort_field = "Id", string sort = "desc")
        {
            return Ok(employeeService.GetAllEmployees(limit, page, search, sort_field, sort));
        }

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
            if (employeeDto == null) return NotFound();

            return Ok(employeeDto);
        }

        /// <summary>
        /// Creates a new Employee item.
        /// </summary>
        /// <returns>Status 201 and created EmployeeDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Employee/Create
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
            return Created("/Employee/Create", employeeService.CreateEmployee(employeeDto));
        }

        /// <summary>
        /// Updates an existing Employee item.
        /// </summary>
        /// <returns>Status 200 and updated EmployeeDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/Employee/Update
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
            if (employeeService.GetEmployeeById(employeeDto.Id) == null) return NotFound();

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
            if (employeeToDelete == null) return NotFound();
            employeeService.DeleteEmployee(id);

            return Ok(employeeToDelete);
        }
    }
}
