using CoreWebApi.Library;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IMailSubscriptionService : IBaseService<MailSubscriptionDto>
    {
        Task<ISearchResult<MailSubscriptionDto>> GetAsync(int limit, int page, OrderType order);
    }
}
