using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class MailSubscriberService : BaseService<MailSubscriber>, IMailSubscriberService
    {
        private readonly IRepository<MailSubscription> subscriptionRepository;

        public MailSubscriberService(
            IMapper mapper,
            IRepository<MailSubscriber> repository,
            IRepository<MailSubscription> subscriptionRepository) : base(mapper, repository) =>
            this.subscriptionRepository = subscriptionRepository;

        public IEnumerable<MailSubscriberDto> GetAllMailSubscribers(int page, string sort, int limit)
        {
            // sorting only by Email
            Func<IQueryable<MailSubscriber>, IOrderedQueryable<MailSubscriber>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Email) : orderBy = q => q.OrderByDescending(s => s.Email);
            var subscriberDtos = mapper.Map<IEnumerable<MailSubscriberDto>>(repository.GetAll(limit, page, null, orderBy));

            if (((List<MailSubscriberDto>)subscriberDtos).Count > 0)
                foreach (var ms in subscriberDtos)
                    ms.MailSubscriptionDto = mapper.Map<MailSubscriptionDto>(subscriptionRepository.Get(ms.MailSubscriptionId));

            return subscriberDtos;
        }

        public async Task<MailSubscriberDto> GetByIdAsync(int id) => mapper.Map<MailSubscriberDto>(await repository.GetAsync(id));

        public async Task<MailSubscriberDto> CreateAsync(MailSubscriberDto mailSubscriberDto)
        {
            var subscriber = mapper.Map<MailSubscriber>(mailSubscriberDto);

            return mapper.Map<MailSubscriberDto>(await repository.CreateAsync(subscriber));
        }

        public async Task UpdateAsync(MailSubscriberDto mailSubscriberDto) =>
            await repository.UpdateAsync(mapper.Map<MailSubscriber>(mailSubscriberDto));

        public async Task<MailSubscriberDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument)
        {
            var mailSubscriber = await repository.GetAsync(id);
            patchDocument.ApplyTo(mailSubscriber);
            return mapper.Map<MailSubscriberDto>(await repository.SaveAsync(mailSubscriber));
        }

        public async Task DeleteAsync(int id) => await repository.DeleteAsync(id);

        public async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };
            return await repository.IsExistAsync("EXEC @returnVal=sp_checkMailSubscriberById @id, @returnVal", parameters);
        }

        public async Task<bool> IsExistAsync(int mailSubscriptionId, string email)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@mailSubscriptionId", SqlDbType.Int) { Value = mailSubscriptionId },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output},
                   new SqlParameter("@email", SqlDbType.NVarChar) { Value = email }
                };
            return await repository.IsExistAsync("EXEC @returnVal=sp_checkMailSubscriberBySubscriptionIdAndEmail @mailSubscriptionId, @returnVal, @email", parameters);
        }
    }
}
