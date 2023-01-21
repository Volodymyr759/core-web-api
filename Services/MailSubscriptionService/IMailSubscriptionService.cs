using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IMailSubscriptionService
    {
        Task<SearchResult<MailSubscriptionDto>> GetMailSubscriptionsSearchResultAsync(int limit, int page, OrderType order);
        MailSubscriptionDto GetMailSubscriptionById(int id);
        MailSubscriptionDto CreateMailSubscription(MailSubscriptionDto mailSubscriptionDto);
        MailSubscriptionDto UpdateMailSubscription(MailSubscriptionDto mailSubscriptionDto);
        MailSubscriptionDto DeleteMailSubscription(int id);
    }
}
