using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library.Enums;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models;
using CoreWebApi.Services.MailSubscriberService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services.MailSubscriptionService
{
    public class MailSubscriptionService : IMailSubscriptionService
    {
        private readonly IMapper mapper;
        private readonly IRepository<MailSubscription> subscriptionRepository;
        private readonly IRepository<MailSubscriber> subscriberRepository;

        public MailSubscriptionService(
            IMapper mapper,
            IRepository<MailSubscription> subscriptionRepository,
            IRepository<MailSubscriber> subscriberRepository)
        {
            this.mapper = mapper;
            this.subscriptionRepository = subscriptionRepository;
            this.subscriberRepository = subscriberRepository;
        }

        public async Task<SearchResult<MailSubscriptionDto>> GetMailSubscriptionsSearchResultAsync(int limit, int page, OrderType order)
        {
            // sorting only by Title
            Func<IQueryable<MailSubscription>, IOrderedQueryable<MailSubscription>> orderBy = null;
            orderBy = order == OrderType.Ascending ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);

            var mailSubscriptions = await subscriptionRepository.GetAllAsync(null, orderBy);

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

        public MailSubscriptionDto GetMailSubscriptionById(int id)
        {
            var subscriptionDto = mapper.Map<MailSubscriptionDto>(subscriptionRepository.Get(t => t.Id == id));
            subscriptionDto.MailSubscriberDtos = mapper.Map<IEnumerable<MailSubscriberDto>>(subscriberRepository.GetAll());

            return subscriptionDto;
        }

        public MailSubscriptionDto CreateMailSubscription(MailSubscriptionDto mailSubscriptionDto)
        {
            var subscription = mapper.Map<MailSubscription>(mailSubscriptionDto);

            return mapper.Map<MailSubscriptionDto>(subscriptionRepository.Create(subscription));
        }

        public MailSubscriptionDto UpdateMailSubscription(MailSubscriptionDto mailSubscriptionDto)
        {
            var subscription = mapper.Map<MailSubscription>(mailSubscriptionDto);

            return mapper.Map<MailSubscriptionDto>(subscriptionRepository.Update(subscription));
        }

        public MailSubscriptionDto DeleteMailSubscription(int id)
        {
            return mapper.Map<MailSubscriptionDto>(subscriptionRepository.Delete(id));
        }
    }
}
