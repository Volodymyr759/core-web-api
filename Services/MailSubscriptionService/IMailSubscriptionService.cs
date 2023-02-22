using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IMailSubscriptionService
    {
        Task<SearchResult<MailSubscriptionDto>> GetMailSubscriptionsSearchResultAsync(int limit, int page, OrderType order);
        Task<MailSubscriptionDto> GetMailSubscriptionByIdAsync(int id);
        Task<MailSubscriptionDto> CreateMailSubscriptionAsync(MailSubscriptionDto mailSubscriptionDto);
        Task UpdateMailSubscriptionAsync(MailSubscriptionDto mailSubscriptionDto);
        Task DeleteMailSubscriptionAsync(int id);
        Task<bool> IsExistAsync(int id);
    }
}
