using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library;
using CoreWebApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class MailSubscriptionService : AppBaseService<MailSubscription, MailSubscriptionDto>, IMailSubscriptionService
    {
        public MailSubscriptionService(
            IMapper mapper,
            IRepository<MailSubscription> repository,
            ISearchResult<MailSubscriptionDto> searchResult,
            IServiceResult<MailSubscription> serviceResult) : base(mapper, repository, searchResult, serviceResult) { }

        public async Task<ISearchResult<MailSubscriptionDto>> GetAsync(int limit, int page, OrderType order)
        {
            // sorting
            Func<IQueryable<MailSubscription>, IOrderedQueryable<MailSubscription>> orderBy = null;
            if (order != OrderType.None)
                orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);

            // adding navigation properties
            Expression<Func<MailSubscription, object>> includeSubscription = ms => ms.MailSubscribers;
            Expression<Func<MailSubscription, object>>[] navigationProperties =
                new Expression<Func<MailSubscription, object>>[] { includeSubscription };

            return await Search(limit: limit, page: page, order: order, orderBy: orderBy, navigationProperties: navigationProperties);
        }

        public override async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await Repository.IsExistAsync("EXEC @returnVal=sp_checkMailSubscriptionById @id, @returnVal", parameters);
        }

        new public Task<MailSubscriptionDto> PartialUpdateAsync(int id, JsonPatchDocument<object> patchDocument)
        {
            throw new NotImplementedException();
        }
    }
}
