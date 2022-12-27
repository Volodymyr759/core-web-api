using CoreWebApi.Services.MailSubscriberService;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers.MailSubscriber
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    public class MailSubscriberController : ControllerBase
    {
        private readonly IMailSubscriberService mailSubscriberService;

        public MailSubscriberController(IMailSubscriberService mailSubscriberService)
        {
            this.mailSubscriberService = mailSubscriberService;
        }

        [HttpGet]
        public IActionResult GetMailSubscribers(int page, string sort, int limit)
        {
            //return Ok(mailSubscriberService.GetAllMailSubscribers(page, sort, limit));
            var subscribers = mailSubscriberService.GetAllMailSubscribers(page, sort, limit);

            return Ok(subscribers);

        }

    }
}
