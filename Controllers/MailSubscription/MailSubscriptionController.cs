using CoreWebApi.Services.MailSubscriptionService;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers.MailSubscription
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class MailSubscriptionController : ControllerBase
    {
        private readonly IMailSubscriptionService mailSubscriptionService;

        public MailSubscriptionController(IMailSubscriptionService mailSubscriptionService)
        {
            this.mailSubscriptionService = mailSubscriptionService;
        }



    }
}
