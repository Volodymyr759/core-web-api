using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class MailSubscriberService : BaseService<MailSubscriber>, IMailSubscriberService
    {
        public MailSubscriberService(IMapper mapper, IRepository<MailSubscriber> repository) : base(mapper, repository) { }

        public async Task<SearchResult<MailSubscriberDto>> GetMailSubscribersSearchResultAsync(int page, OrderType order, int limit)
        {
            // sorting only by Email
            Func<IQueryable<MailSubscriber>, IOrderedQueryable<MailSubscriber>> orderBy = null;
            orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Email) : orderBy = q => q.OrderByDescending(s => s.Email);
            var mailSubscribers = await repository.GetAllAsync(null, orderBy);

            return new SearchResult<MailSubscriberDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)mailSubscribers.Count() / limit)),
                SearchCriteria = "",
                TotalItemCount = mailSubscribers.Count(),
                ItemList = (List<MailSubscriberDto>)mapper.Map<IEnumerable<MailSubscriberDto>>(mailSubscribers.Skip((page - 1) * limit).Take(limit))
            };
        }

        public async Task<MailSubscriberDto> GetByIdAsync(int id)
        {
            Expression<Func<MailSubscriber, bool>> searchQuery = ms => ms.Id == id;
            Expression<Func<MailSubscriber, object>> include = ms => ms.MailSubscription;
            var subscriber = await repository.GetAsync(searchQuery, include);

            return mapper.Map<MailSubscriberDto>(subscriber);
        }

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
