using CoreWebApi.Library.ResponseError;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers
{
    [ApiController, Authorize, Produces("application/json"), Route("api/[controller]/[action]"), ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class MailSubscriberController : ControllerBase
    {
        private readonly IMailSubscriberService mailSubscriberService;
        private IResponseError responseBadRequestError;
        private IResponseError responseNotFoundError;

        public MailSubscriberController(IMailSubscriberService mailSubscriberService)
        {
            this.mailSubscriberService = mailSubscriberService;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("");
        }

        /// <summary>
        /// Gets a list of MailSubscriberDto's with values for pagination (page number, limit) and sorting by Email.
        /// </summary>
        /// <param name="page" default="1">requested page</param>
        /// <param name="sort" default="asc">sort direction: asc or desc</param>
        /// <param name="limit" default="10">number of items per page</param>
        /// <returns>Status 200 and list of MailSubscriberDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/MailSubscriber/GetAll;page=1;sort=asc;limit=3
        ///     
        /// </remarks>
        /// <response code="200">list of MailSubscriberDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll(int page = 1, string sort = "asc", int limit = 10)
        {
            return Ok(mailSubscriberService.GetAllMailSubscribers(page, sort, limit));
        }

        /// <summary>
        /// Gets a specific MailSubscriberDto Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>OK and MailSubscriberDto</returns>
        /// <response code="200">Returns the requested MailSubscriberDto item</response>
        /// <response code="404">If the mail subscriber with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById([FromRoute] int id)
        {
            var mailSubscriberDto = mailSubscriberService.GetMailSubscriberById(id);
            if (mailSubscriberDto == null)
            {
                responseNotFoundError.Title = "Mail Subscriber Not Found.";
                return NotFound(responseNotFoundError);
            }

            return Ok(mailSubscriberDto);
        }

        /// <summary>
        /// Creates a new MailSubscriberDto item.
        /// </summary>
        /// <returns>Status 201 and created MailSubscriberDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/nailsubscriber/subscribe
        ///     {
        ///        "Email" = "test1@gmail.com",
        ///        "IsSubscribed": "true",
        ///        "MailSubscriptionId": "1"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created MailSubscriberDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Subscribe([FromBody] MailSubscriberDto mailSubscriberDto)
        {
            if (!ModelState.IsValid)
            {
                responseBadRequestError.Title = "Wrong mail subscriber - data.";
                return BadRequest(responseBadRequestError);
            }
            return Created("/api/nailsubscriber/subscribe", mailSubscriberService.CreateMailSubscriber(mailSubscriberDto));
        }

        /// <summary>
        /// Partly updates an existing MailSubscriber Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <param name="patchDocument">Json Patch Document</param>
        /// <returns>Status 200 and updated MailSubscriberDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/MailSubscriber/Unsubscribe/{id}
        ///     {
        ///        "op": "replace",
        ///        "path": "/issubscribed",
        ///        "value": "false"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated MailSubscriberDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the mail subscriber with given id not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Unsubscribe([FromRoute] int id, [FromBody] JsonPatchDocument<MailSubscriberDto> patchDocument)
        {
            var mailSubscriberDto = mailSubscriberService.GetMailSubscriberById(id);
            if (mailSubscriberDto == null)
            {
                responseNotFoundError.Title = "Mail Subscriber Not Found.";
                return NotFound(responseNotFoundError);
            }

            patchDocument.ApplyTo(mailSubscriberDto, ModelState);
            if (!TryValidateModel(mailSubscriberDto)) return ValidationProblem(ModelState);

            return Ok(mailSubscriberService.UpdateMailSubscriber(mailSubscriberDto));
        }

        /// <summary>
        /// Deletes a MailSubscriber Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200 and deleted MailSubscriberDto object</returns>
        /// <response code="200">Returns the deleted MailSubscriberDto item</response>
        /// <response code="404">If the mail subscriber with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int id)
        {
            var mailSubscriberToDelete = mailSubscriberService.GetMailSubscriberById(id);
            if (mailSubscriberToDelete == null)
            {
                responseNotFoundError.Title = "Mail Subscriber Not Found.";
                return NotFound(responseNotFoundError);
            }
            mailSubscriberService.DeleteMailSubsriber(id);

            return Ok(mailSubscriberToDelete);
        }
    }
}
