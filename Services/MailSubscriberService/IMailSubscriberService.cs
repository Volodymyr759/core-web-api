using System.Collections.Generic;

namespace CoreWebApi.Services
{
    public interface IMailSubscriberService
    {
        IEnumerable<MailSubscriberDto> GetAllMailSubscribers(int page, string sort, int limit);
        MailSubscriberDto GetMailSubscriberById(int id);
        IEnumerable<MailSubscriptionDto> GetSubscriptionsBySubscribersEmail(int page, string sort, int limit);
        MailSubscriberDto CreateMailSubscriber(MailSubscriberDto mailSubscriberDto);
        MailSubscriberDto UpdateMailSubscriber(MailSubscriberDto mailSubscriberDto);
        MailSubscriberDto DeleteMailSubsriber(int id);
    }
}
