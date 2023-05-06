using CoreWebApi.Library;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CandidateController : AppControllerBase
    {
        private readonly ICandidateService candidateService;
        private readonly IVacancyService vacancyService;

        public CandidateController(ICandidateService candidateService, IVacancyService vacancyService)
        {
            this.candidateService = candidateService;
            this.vacancyService = vacancyService;
        }

        /// <summary>
        /// Gets a list of CandidateDto's with pagination params and values for search and sorting.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="search">Part of FullName for searching</param>
        /// <param name="candidateStatus">Filter for isDismissed property: 0 - Active, 1 - Dismissed, 2 - All</param>
        /// <param name="vacancyId">Filter for candidates by VacancyId</param>
        /// <param name="sortField">Field name for sorting</param>
        /// <param name="order">sort direction: 0 - Ascending or 1 - Descending, 2 - None</param>
        /// <returns>Status 200 and list of CandidateDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/candidate/get/?limit=10&amp;page=1&amp;search=&amp;candidateStatus=2&amp;vacancyId=&amp;sort_field=Id&amp;order=0
        ///     
        /// </remarks>
        /// <response code="200">list of CandidateDto's</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, string search, CandidateStatus candidateStatus, int? vacancyId, string sortField, OrderType order) =>
            Ok(await candidateService.GetAsync(limit, page, search, candidateStatus, vacancyId, sortField, order));

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
        public async Task<IActionResult> GetAsync([FromRoute] int id)
        {
            var candidateDto = await candidateService.GetAsync(id);
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
        [HttpPost]
        [Authorize(Roles = "Registered, Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] CandidateDto candidateDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            return Created("/api/candidate/create", await candidateService.CreateAsync(candidateDto));
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
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] CandidateDto candidateDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            if (await IsExistAsync(candidateDto.Id) == false) return NotFound(responseNotFoundError);
            await candidateService.UpdateAsync(candidateDto);
            if (candidateDto.VacancyDto == null) candidateDto.VacancyDto = await vacancyService.GetAsync(candidateDto.VacancyId);

            return Ok(candidateDto);
        }

        /// <summary>
        /// Partly updates an existing Candidate item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <param name="patchDocument">Json Patch Document as array of operations</param>
        /// <returns>Status 200 and updated CandidateDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/candidate/partialcandidateupdate/{id}
        ///     [
        ///         {
        ///             "op": "replace",
        ///             "path": "/isdismissed",
        ///             "value": "true"
        ///         }
        ///     ]
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated CandidateDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the candidate with given id not found</response>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartialCandidateUpdateAsync(int id, JsonPatchDocument<object> patchDocument)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            try
            {
                return Ok(await candidateService.PartialUpdateAsync(id, patchDocument));
            }
            catch
            {
                return BadRequest(responseBadRequestError);
            }
        }

        /// <summary>
        /// Deletes an Candidate Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/candidate/delete/3
        ///     
        /// </remarks>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted CandidateDto object</returns>
        /// <response code="200">Returns the deleted CandidateDto item</response>
        /// <response code="404">If the candidate with given id not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Registered")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            await candidateService.DeleteAsync(id);

            return Ok();
        }

        private async Task<bool> IsExistAsync(int id) => await candidateService.IsExistAsync(id);
    }
}
