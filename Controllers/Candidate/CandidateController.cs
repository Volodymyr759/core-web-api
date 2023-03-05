using CoreWebApi.Library.Enums;
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
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService candidateService;
        private readonly IResponseError responseBadRequestError;
        private readonly IResponseError responseNotFoundError;

        public CandidateController(ICandidateService candidateService)
        {
            this.candidateService = candidateService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("Wrong candidate data.");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("Candidate Not Found.");
        }

        /// <summary>
        /// Gets a list of CandidateDto's with pagination params and values for search and sorting.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="search">Part of FullName for searching</param>
        /// <param name="sortField">field name for sorting</param>
        /// <param name="order">sort direction: 0 - Ascending or 1 - Descending, 2 - None</param>
        /// <returns>Status 200 and list of CandidateDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Candidate/get/?limit=10&amp;page=1&amp;search=&amp;sort_field=Id&amp;order=0
        ///     
        /// </remarks>
        /// <response code="200">list of CandidateDto's</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, string search, string sortField, OrderType order) =>
            Ok(await candidateService.GetCandidatesSearchResultAsync(limit, page, search, sortField, order));

        /// <summary>
        /// Gets a list of CandidateDto's for public pages.
        /// </summary>
        /// <param name="page">Requested page</param>
        /// <returns>Status 200 and list of CandidateDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Candidate/getpublic/?page=1
        ///     
        /// </remarks>
        /// <response code="200">list of CandidateDto's</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicAsync(int page) =>
            Ok(await candidateService.GetCandidatesSearchResultAsync(limit: 10, page, search: "", sortField: "Id", order: OrderType.Descending));

        /// <summary>
        /// Gets a specific CandidateDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and CandidateDto</returns>
        /// <response code="200">Returns the requested CandidateDto item</response>
        /// <response code="404">If the candidate with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var candidateDto = await candidateService.GetCandidateByIdAsync(id);
            if (candidateDto == null) return NotFound(responseNotFoundError);

            return Ok(candidateDto);
        }

        /// <summary>
        /// Creates a new Candidate item.
        /// </summary>
        /// <returns>Status 201 and created CandidateDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/candidate/create
        ///     {
        ///        "FullName": "Sindy Crowford",
        ///        "Email": "sindy@gmail.com",
        ///        "Phone": "+1234567891",
        ///        "Notes": "Test note 1",
        ///        "IsDismissed": "false",
        ///        "JoinedAt": "2022/12/31",
        ///        "VacancyId": "1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created CandidateDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost, Authorize(Roles = "Registered, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] CandidateDto candidateDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            return Created("/api/candidate/create", await candidateService.CreateCandidateAsync(candidateDto));
        }

        /// <summary>
        /// Updates an existing Candidate item.
        /// </summary>
        /// <returns>Status 200 and updated CandidateDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/Candidate/Update
        ///     {
        ///        "Id": "1",
        ///        "FullName": "Sindy Crowford",
        ///        "Email": "sindy@gmail.com",
        ///        "Phone": "+1234567891",
        ///        "Notes": "Test note 1",
        ///        "IsDismissed": "false",
        ///        "JoinedAt": "20221231",
        ///        "VacancyId": "1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated CandidateDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the candidate with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] CandidateDto candidateDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            if (await IsExistAsync(candidateDto.Id) == false) return NotFound(responseNotFoundError);
            await candidateService.UpdateCandidateAsync(candidateDto);

            return Ok(candidateDto);
        }

        /// <summary>
        /// Deletes an Candidate Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted CandidateDto object</returns>
        /// <response code="200">Returns the deleted CandidateDto item</response>
        /// <response code="404">If the candidate with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            await candidateService.DeleteCandidateAsync(id);

            return Ok();
        }

        private async Task<bool> IsExistAsync(int id) => await candidateService.IsExistAsync(id);
    }
}
