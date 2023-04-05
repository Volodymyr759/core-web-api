using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IMailSubscriptionService : IBaseService<MailSubscriptionDto>
    {
        Task<SearchResult<MailSubscriptionDto>> GetMailSubscriptionsSearchResultAsync(int limit, int page, OrderType order);
    }
}
