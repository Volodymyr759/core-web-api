using CoreWebApi.Services.CandidateService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers.Candidate
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            this.candidateService = candidateService;
        }

        /// <summary>
        /// Gets a list of CandidateDto's with pagination params and values for search and sorting.
        /// </summary>
        /// <param name="limit" default="10">number of items per page</param>
        /// <param name="page" default="1">requested page</param>
        /// <param name="search">part of FullName for searching</param>
        /// <param name="sort_field" default="id">field name for sorting</param>
        /// <param name="sort" default="desc">sort direction: asc or desc</param>
        /// <returns>Status 200 and list of CandidateDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/Candidate/GetAll/?limit=10&amp;page=1&amp;search=j&amp;sort_field=Id&amp;sort=desc
        ///     
        /// </remarks>
        /// <response code="200">list of CandidateDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll(int limit = 10, int page = 1, string search = "", string sort_field = "Id", string sort = "desc")
        {
            return Ok(candidateService.GetAllCandidates(limit, page, search, sort_field, sort));
        }

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
        public IActionResult GetById([FromRoute] int id)
        {
            var candidateDto = candidateService.GetCandidateById(id);
            if (candidateDto == null) return NotFound();

            return Ok(candidateDto);
        }

        /// <summary>
        /// Creates a new Candidate item.
        /// </summary>
        /// <returns>Status 201 and created CandidateDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Candidate/Create
        ///     {
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
        /// <response code="201">Returns the newly created CandidateDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] CandidateDto candidateDto)
        {
            return Created("/Candidate/Create", candidateService.CreateCandidate(candidateDto));
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
        public IActionResult Update([FromBody] CandidateDto candidateDto)
        {
            if (candidateService.GetCandidateById(candidateDto.Id) == null) return NotFound();

            return Ok(candidateService.UpdateCandidate(candidateDto));
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
        public IActionResult Delete([FromRoute] int id)
        {
            var candidateToDelete = candidateService.GetCandidateById(id);
            if (candidateToDelete == null) return NotFound();
            candidateService.DeleteCandidate(id);

            return Ok(candidateToDelete);
        }
    }
}
