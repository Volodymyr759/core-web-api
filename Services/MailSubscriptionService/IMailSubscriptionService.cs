using System.Collections.Generic;

namespace CoreWebApi.Services.MailSubscriptionService
{
    public interface IMailSubscriptionService
    {
        IEnumerable<MailSubscriptionDto> GetAllMailSubscriptions(int page, string sort, int limit);
        MailSubscriptionDto GetMailSubscriptionById(int id);
        MailSubscriptionDto CreateMailSubscription(MailSubscriptionDto mailSubscriptionDto);
        MailSubscriptionDto UpdateMailSubscription(MailSubscriptionDto mailSubscriptionDto);
        MailSubscriptionDto DeleteMailSubscription(int id);
    }
}
