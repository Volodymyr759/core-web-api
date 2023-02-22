using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class MailSubscriptionService : IMailSubscriptionService
    {
        private readonly IMapper mapper;
        private readonly IRepository<MailSubscription> repository;

        public MailSubscriptionService(
            IMapper mapper,
            IRepository<MailSubscription> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public async Task<SearchResult<MailSubscriptionDto>> GetMailSubscriptionsSearchResultAsync(int limit, int page, OrderType order)
        {
            // sorting only by Title
            Func<IQueryable<MailSubscription>, IOrderedQueryable<MailSubscription>> orderBy = null;
            orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);

            var mailSubscriptions = await repository.GetAllAsync(null, orderBy);

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

        public async Task<MailSubscriptionDto> GetMailSubscriptionByIdAsync(int id) => mapper.Map<MailSubscriptionDto>(await repository.GetAsync(id));

        public async Task<MailSubscriptionDto> CreateMailSubscriptionAsync(MailSubscriptionDto mailSubscriptionDto)
        {
            var subscription = mapper.Map<MailSubscription>(mailSubscriptionDto);

            return mapper.Map<MailSubscriptionDto>(await repository.CreateAsync(subscription));
        }

        public async Task UpdateMailSubscriptionAsync(MailSubscriptionDto mailSubscriptionDto) =>
            await repository.UpdateAsync(mapper.Map<MailSubscription>(mailSubscriptionDto));

        public async Task DeleteMailSubscriptionAsync(int id) => await repository.DeleteAsync(id);

        public async Task<bool> IsExistAsync(int id)
        {
            SqlParameter[] parameters =
                {
                   new SqlParameter("@id", SqlDbType.Int) { Value = id },
                   new SqlParameter("@returnVal", SqlDbType.Int) {Direction = ParameterDirection.Output}
                };

            return await repository.IsExistAsync("EXEC @returnVal=sp_checkMailSubscriptionById @id, @returnVal", parameters);
        }
    }
}
