using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.MailSubscriberService;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<MailSubscriptionDto> GetAllMailSubscriptions(int page, string sort, int limit)
        {
            // sorting only by Title
            Func<IQueryable<MailSubscription>, IOrderedQueryable<MailSubscription>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);

            return mapper.Map<IEnumerable<MailSubscriptionDto>>(subscriptionRepository.GetAll(limit, page, null, orderBy));
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
