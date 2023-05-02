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
    public class MailSubscriptionService : BaseService<MailSubscription>, IMailSubscriptionService
    {
        public MailSubscriptionService(IMapper mapper, IRepository<MailSubscription> repository) : base(mapper, repository) { }

        public async Task<SearchResult<MailSubscriptionDto>> GetMailSubscriptionsSearchResultAsync(int limit, int page, OrderType order)
        {
            // sorting only by Title
            Func<IQueryable<MailSubscription>, IOrderedQueryable<MailSubscription>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);
            Expression<Func<MailSubscription, object>> include = ms => ms.MailSubscribers;
            var mailSubscriptions = await repository.GetAllAsync(null, orderBy, include);

            return new SearchResult<MailSubscriptionDto>
            {
                CurrentPageNumber = page,
                Order = order,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)mailSubscriptions.Count() / limit)),
                SearchCriteria = "",
                TotalItemCount = mailSubscriptions.Count(),
                ItemList = (List<MailSubscriptionDto>)mapper.Map<IEnumerable<MailSubscriptionDto>>(mailSubscriptions.Skip((page - 1) * limit).Take(limit))
            };
        }

        public async Task<MailSubscriptionDto> GetByIdAsync(int id)
        {
            Expression<Func<MailSubscription, bool>> searchQuery = ms => ms.Id == id;
            Expression<Func<MailSubscription, object>> include = ms => ms.MailSubscribers;
            var subscription = await repository.GetAsync(searchQuery, include);

            return mapper.Map<MailSubscriptionDto>(subscription);
        }

        public async Task<MailSubscriptionDto> CreateAsync(MailSubscriptionDto mailSubscriptionDto)
        {
            var subscription = mapper.Map<MailSubscription>(mailSubscriptionDto);

            return mapper.Map<MailSubscriptionDto>(await repository.CreateAsync(subscription));
        }

        public async Task UpdateAsync(MailSubscriptionDto mailSubscriptionDto) =>
            await repository.UpdateAsync(mapper.Map<MailSubscription>(mailSubscriptionDto));

        public async Task DeleteAsync(int id) => await repository.DeleteAsync(id);

        public async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await repository.IsExistAsync("EXEC @returnVal=sp_checkMailSubscriptionById @id, @returnVal", parameters);
        }

        public Task<MailSubscriptionDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument)
        {
            throw new NotImplementedException();
        }
    }
}
