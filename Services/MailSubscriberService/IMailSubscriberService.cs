using CoreWebApi.Services.MailSubscriptionService;
using System.Collections.Generic;

namespace CoreWebApi.Services.MailSubscriberService
{
    public interface IMailSubscriberService
    {
        IEnumerable<MailSubscriberDto> GetAllMailSubscribers(int page, string sort, int limit);
        MailSubscriberDto GetMailSubscriberById(int id);
        IEnumerable<MailSubscriptionDto> GetSubscriptionsBySubscribersEmail(int page, string sort, int limit);
        MailSubscriberDto Subscribe(MailSubscriberDto mailSubscriberDto);
        MailSubscriberDto Unsubscribe(int id);
        MailSubscriberDto DeleteMailSubsriber(int id);
    }
}
