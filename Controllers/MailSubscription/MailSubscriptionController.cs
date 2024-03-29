﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using CoreWebApi.Services;
using CoreWebApi.Library;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class MailSubscriptionController : AppControllerBase
    {
        private readonly IMailSubscriptionService mailSubscriptionService;

        public MailSubscriptionController(IMailSubscriptionService mailSubscriptionService) =>
            this.mailSubscriptionService = mailSubscriptionService;

        /// <summary>
        /// Gets a list of MailSubscriptionDto's with values for pagination (limit, page number) and sorting by Title.
        /// </summary>
        /// <param name="limit">Number of items per page</param>
        /// <param name="page">Requested page</param>
        /// <param name="order">Sort direction: 0 - Ascending or 1 - Descending, 2 - None</param>
        /// 
        /// <returns>Status 200 and list of MailSubscriptionDto's</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/mailsubscription/get?limit=3&amp;page=1&amp;order=0;
        ///     
        /// </remarks>
        /// <response code="200">list of MailSubscriptionDto's</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(int limit, int page, OrderType order) =>
            Ok(await mailSubscriptionService.GetAsync(limit, page, order));

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
        public async Task<IActionResult> GetAsync([FromRoute] int id)
        {
            var mailSubscriptionDto = await mailSubscriptionService.GetAsync(id);
            if (mailSubscriptionDto == null) return NotFound(responseNotFoundError);

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
        public async Task<IActionResult> CreateAsync([FromBody] MailSubscriptionDto mailSubscriptionDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            return Created("/api/mailsubscription/create", await mailSubscriptionService.CreateAsync(mailSubscriptionDto));
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
        public async Task<IActionResult> UpdateAsync([FromBody] MailSubscriptionDto mailSubscriptionDto)
        {
            if (!ModelState.IsValid) return BadRequest(responseBadRequestError);
            if (await IsExistAsync(mailSubscriptionDto.Id) == false) return NotFound(responseNotFoundError);
            await mailSubscriptionService.UpdateAsync(mailSubscriptionDto);

            return Ok(mailSubscriptionDto);
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
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (await IsExistAsync(id) == false) return NotFound(responseNotFoundError);
            await mailSubscriptionService.DeleteAsync(id);

            return Ok();
        }

        private async Task<bool> IsExistAsync(int id) => await mailSubscriptionService.IsExistAsync(id);
    }
}
