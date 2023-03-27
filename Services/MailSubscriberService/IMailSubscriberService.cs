using CoreWebApi.Library.SearchResult;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IMailSubscriberService
    {
        IEnumerable<MailSubscriberDto> GetAllMailSubscribers(int page, string sort, int limit);
        Task<MailSubscriberDto> GetMailSubscriberByIdAsync(int id);
        Task<SearchResult<MailSubscriptionDto>> GetSubscriptionsBySubscribersEmailAsync(int page, string sort, int limit);
        Task<MailSubscriberDto> CreateMailSubscriberAsync(MailSubscriberDto mailSubscriberDto);
        Task UpdateMailSubscriberAsync(MailSubscriberDto mailSubscriberDto);
        Task DeleteMailSubsriberAsync(int id);
        Task<MailSubscriberDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument);
        Task<bool> IsExistAsync(int id);
        Task<bool> IsExistAsync(int mailSubscriptionId, string email);
    }
}
