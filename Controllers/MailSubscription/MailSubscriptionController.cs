using CoreWebApi.Services.MailSubscriptionService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using CoreWebApi.Controllers.ResponseError;

namespace CoreWebApi.Controllers.MailSubscription
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]"), ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class MailSubscriptionController : ControllerBase
    {
        private readonly IMailSubscriptionService mailSubscriptionService;
        private IResponseError responseBadRequestError;
        private IResponseError responseNotFoundError;

        public MailSubscriptionController(IMailSubscriptionService mailSubscriptionService)
        {
            this.mailSubscriptionService = mailSubscriptionService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("");
        }

        /// <summary>
        /// Gets a list of MailSubscriptionDto's with values for pagination (page number, limit) and sorting by Title.
        /// </summary>
        /// <param name="page" default="1">requested page</param>
        /// <param name="sort" default="asc">sort direction: asc or desc</param>
        /// <param name="limit" default="10">number of items per page</param>
        /// <returns>Status 200 and list of MailSubscriptionDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/MailSubscription/GetAll;page=1;sort=asc;limit=3
        ///     
        /// </remarks>
        /// <response code="200">list of MailSubscriptionDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll(int page = 1, string sort = "asc", int limit = 10)
        {
            return Ok(mailSubscriptionService.GetAllMailSubscriptions(page, sort, limit));
        }

        /// <summary>
        /// Gets a specific MailSubscriptionDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and MailSubscriptionDto</returns>
        /// <response code="200">Returns the requested MailSubscriptionDto item</response>
        /// <response code="404">If the mail subscription with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById([FromRoute] int id)
        {
            var mailSubscriptionDto = mailSubscriptionService.GetMailSubscriptionById(id);
            if (mailSubscriptionDto == null)
            {
                responseNotFoundError.Title = "Mail Subscription Not Found.";
                return NotFound(responseNotFoundError);
            }

            return Ok(mailSubscriptionDto);
        }

        /// <summary>
        /// Creates a new MailSubscription item.
        /// </summary>
        /// <returns>Status 201 and created MailSubscriptionDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/mailsubscription/create
        ///     {
        ///        "Title": "Company News",
        ///        "Content": "Test content 1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created MailSubscriptionDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] MailSubscriptionDto mailSubscriptionDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong mail subscription - data.";
                return BadRequest(responseBadRequestError);
            }
            return Created("/api/mailsubscription/create", mailSubscriptionService.CreateMailSubscription(mailSubscriptionDto));
        }

        /// <summary>
        /// Updates an existing MailSubscription item.
        /// </summary>
        /// <returns>Status 200 and updated MailSubscriptionDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/MailSubscription/Update
        ///     {
        ///        "Id": "1",
        ///        "Title": "Company News",
        ///        "Content": "Test content 1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated MailSubscriptionDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the mail subscription with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update([FromBody] MailSubscriptionDto mailSubscriptionDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong mail subscription - data.";
                return BadRequest(responseBadRequestError);
            }
            if (mailSubscriptionService.GetMailSubscriptionById(mailSubscriptionDto.Id) == null)
            {
                responseNotFoundError.Title = "Mail Subscription Not Found.";
                return NotFound(responseNotFoundError);
            }

            return Ok(mailSubscriptionService.UpdateMailSubscription(mailSubscriptionDto));
        }

        /// <summary>
        /// Deletes a MailSubscription Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted MailSubscriptionDto object</returns>
        /// <response code="200">Returns the deleted MailSubscriptionDto item</response>
        /// <response code="404">If the mail subscription with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int id)
        {
            var mailSubscriptionToDelete = mailSubscriptionService.GetMailSubscriptionById(id);
            if (mailSubscriptionToDelete == null)
            {
                responseNotFoundError.Title = "Mail Subscription Not Found.";
                return NotFound(responseNotFoundError);
            }
            mailSubscriptionService.DeleteMailSubscription(id);

            return Ok(mailSubscriptionToDelete);
        }

    }
}
