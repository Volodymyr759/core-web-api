using CoreWebApi.Library;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IMailSubscriberService : IBaseService<MailSubscriberDto>
    {
        Task<ISearchResult<MailSubscriberDto>> GetAsync(int limit, int page, OrderType order);

        Task<bool> IsExistAsync(int mailSubscriptionId, string email);
    }
}
