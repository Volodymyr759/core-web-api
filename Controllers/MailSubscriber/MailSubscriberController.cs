using CoreWebApi.Library.ResponseError;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class MailSubscriberController : ControllerBase
    {
        private readonly IMailSubscriberService mailSubscriberService;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;
        private readonly IResponseError responseBadRequestError;
        private readonly IResponseError responseNotFoundError;

        public MailSubscriberController(IMailSubscriberService mailSubscriberService,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            this.mailSubscriberService = mailSubscriberService;
            this.emailSender = emailSender;
            this.configuration = configuration;
            responseBadRequestError = ResponseErrorFactory.getBadRequestError("Wrong mail subscriber data.");
            responseNotFoundError = ResponseErrorFactory.getNotFoundError("Mail Subscriber Not Found.");
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
        ///     GET /api/MailSubscriber/get?page=1&amp;sort=asc&amp;limit=3
        ///     
        /// </remarks>
        /// <response code="200">list of MailSubscriberDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get(int page = 1, string sort = "asc", int limit = 10) =>
            Ok(mailSubscriberService.GetAllMailSubscribers(page, sort, limit));

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
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var mailSubscriberDto = await mailSubscriberService.GetByIdAsync(id);
            if (mailSubscriberDto == null) return NotFound(responseNotFoundError);

            return Ok(mailSubscriberDto);
        }

        /// <summary>
        /// Creates a new MailSubscriberDto item.
        /// </summary>
        /// <param name="mailSubscriberDto">MailSubscriberDto object</param>
        /// <returns>Status 201 and created MailSubscriberDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/mailsubscriber/subscribe
        ///     {
        ///        email: "test1@gmail.com",
        ///        isSubscribed: true,
        ///        mailSubscriptionId: 1
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created MailSubscriberDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubscribeAsync([FromBody] MailSubscriberDto mailSubscriberDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            if (await IsExistAsync(mailSubscriberDto.MailSubscriptionId, mailSubscriberDto.Email) == true)
            {
                responseBadRequestError.Title = "Email address " + mailSubscriberDto.Email + " already subscribed.";
                return BadRequest(responseBadRequestError);
            }
            return Created("/api/mailsubscriber/subscribe", await mailSubscriberService.CreateAsync(mailSubscriberDto));
        }

        /// <summary>
        /// Sends message from contact from to the site Administrator.
        /// </summary>
        /// <returns>Status 200 and confirmation message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/mailsubscriber/sendmessagetoadmin
        ///     {
        ///        "senderName": "John Walker",
        ///        "senderEmail": "test1@gmail.com",
        ///        "subject": "Test subject",
        ///        "message": "Test message"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Status Ok and confirmation</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessageToAdminAsync([FromBody] MailMessageDto message)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            await emailSender.SendEmailAsync(
                configuration["EmailSettings:EmailAddress"],
                message.Subject,
                message.SenderName + " has sent from address: " + message.SenderEmail + " the message: " + message.Message);

            return Ok("Your message has delivered to Administrator. We will contact you asap. Thank You!");
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
        public async Task<IActionResult> UnsubscribeAsync([FromRoute] int id, [FromBody] JsonPatchDocument<object> patchDocument)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            try
            {
                return Ok(await mailSubscriberService.PartialUpdateAsync(id, patchDocument));
            }
            catch
            {
                return BadRequest(responseBadRequestError);
            }
        }

        /// <summary>
        /// Deletes a MailSubscriber Item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns the deleted MailSubscriberDto item</response>
        /// <response code="404">If the mail subscriber with given id not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            await mailSubscriberService.DeleteAsync(id);

            return Ok();
        }

        private async Task<bool> IsExistAsync(int id) => await mailSubscriberService.IsExistAsync(id);

        private async Task<bool> IsExistAsync(int mailSubscriptionId, string email) => await mailSubscriberService.IsExistAsync(mailSubscriptionId, email);
    }
}
