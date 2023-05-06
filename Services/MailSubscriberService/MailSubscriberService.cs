using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library;
using CoreWebApi.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class MailSubscriberService : AppBaseService<MailSubscriber, MailSubscriberDto>, IMailSubscriberService
    {
        public MailSubscriberService(
            IMapper mapper,
            IRepository<MailSubscriber> repository,
            ISearchResult<MailSubscriberDto> searchResult,
            IServiceResult<MailSubscriber> serviceResult) : base(mapper, repository, searchResult, serviceResult) { }

        public async Task<ISearchResult<MailSubscriberDto>> GetAsync(int limit, int page, OrderType order)
        {
            // sorting
            Func<IQueryable<MailSubscriber>, IOrderedQueryable<MailSubscriber>> orderBy = null;
            orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Email) : orderBy = q => q.OrderByDescending(s => s.Email);

            return await Search(limit: limit, page: page, order: order, orderBy: orderBy);
        }

        new public async Task<MailSubscriberDto> GetAsync(int id)
        {
            Expression<Func<MailSubscriber, bool>> searchQuery = ms => ms.Id == id;
            Expression<Func<MailSubscriber, object>> includeSubscription = ms => ms.MailSubscription;
            Expression<Func<MailSubscriber, object>>[] navigationProperties =
                new Expression<Func<MailSubscriber, object>>[] { includeSubscription };
            var subscriber = await Repository.GetAsync(searchQuery, navigationProperties);

            return Mapper.Map<MailSubscriberDto>(subscriber);
        }

        public override async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };
            return await Repository.IsExistAsync("EXEC @returnVal=sp_checkMailSubscriberById @id, @returnVal", parameters);
        }

        public async Task<bool> IsExistAsync(int mailSubscriptionId, string email)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@mailSubscriptionId", SqlDbType.Int) { Value = mailSubscriptionId },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output},
                   new SqlParameter("@email", SqlDbType.NVarChar) { Value = email }
                };
            return await Repository.IsExistAsync("EXEC @returnVal=sp_checkMailSubscriberBySubscriptionIdAndEmail @mailSubscriptionId, @returnVal, @email", parameters);
        }
    }
}
