using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
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

        public MailSubscriptionDto GetMailSubscriptionById(int id) => mapper.Map<MailSubscriptionDto>(repository.Get(id));

        public MailSubscriptionDto CreateMailSubscription(MailSubscriptionDto mailSubscriptionDto)
        {
            var subscription = mapper.Map<MailSubscription>(mailSubscriptionDto);

            return mapper.Map<MailSubscriptionDto>(repository.Create(subscription));
        }

        public MailSubscriptionDto UpdateMailSubscription(MailSubscriptionDto mailSubscriptionDto)
        {
            var subscription = mapper.Map<MailSubscription>(mailSubscriptionDto);

            return mapper.Map<MailSubscriptionDto>(repository.Update(subscription));
        }

        public MailSubscriptionDto DeleteMailSubscription(int id) => mapper.Map<MailSubscriptionDto>(repository.Delete(id));
    }
}
