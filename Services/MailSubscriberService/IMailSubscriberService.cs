using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IMailSubscriberService : IBaseService<MailSubscriberDto>
    {
        Task<SearchResult<MailSubscriberDto>> GetMailSubscribersSearchResultAsync(int page, OrderType order, int limit);

        Task<bool> IsExistAsync(int mailSubscriptionId, string email);
    }
}
