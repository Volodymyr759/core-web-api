using CoreWebApi.Library.SearchResult;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IMailSubscriberService : IBaseService<MailSubscriberDto>
    {
        IEnumerable<MailSubscriberDto> GetAllMailSubscribers(int page, string sort, int limit);

        Task<bool> IsExistAsync(int mailSubscriptionId, string email);
    }
}
