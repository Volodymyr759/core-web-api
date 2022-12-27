using System.Collections.Generic;

namespace CoreWebApi.Services.MailSubscriberService
{
    public interface IMailSubscriberService
    {
        IEnumerable<MailSubscriberDto> GetAllMailSubscribers(int page, string sort, int limit);
        MailSubscriberDto GetMailSubscriberById(int id);
        MailSubscriberDto Subscribe(MailSubscriberDto mailSubscriberDto);
        MailSubscriberDto Unsubscribe(int id, bool isSubscribed);
        MailSubscriberDto DeleteMailSubsriber(int id);
    }
}
