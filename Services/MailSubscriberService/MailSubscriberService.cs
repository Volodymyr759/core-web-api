using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWebApi.Services
{
    public class MailSubscriberService : IMailSubscriberService
    {
        private readonly IMapper mapper;
        private readonly IRepository<MailSubscriber> subscriberRepository;
        private readonly IRepository<MailSubscription> subscriptionRepository;

        public MailSubscriberService(
            IMapper mapper,
            IRepository<MailSubscriber> subscriberRepository,
            IRepository<MailSubscription> subscriptionRepository)
        {
            this.mapper = mapper;
            this.subscriberRepository = subscriberRepository;
            this.subscriptionRepository = subscriptionRepository;
        }

        public IEnumerable<MailSubscriberDto> GetAllMailSubscribers(int page, string sort, int limit)
        {
            // sorting only by Email
            Func<IQueryable<MailSubscriber>, IOrderedQueryable<MailSubscriber>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Email) : orderBy = q => q.OrderByDescending(s => s.Email);
            var subscriberDtos = mapper.Map<IEnumerable<MailSubscriberDto>>(subscriberRepository.GetAll(limit, page, null, orderBy));

            if (((List<MailSubscriberDto>)subscriberDtos).Count > 0)
                foreach (var ms in subscriberDtos)
                    ms.MailSubscriptionDto = mapper.Map<MailSubscriptionDto>(subscriptionRepository.Get(ms.MailSubscriptionId));

            return subscriberDtos;
        }

        public MailSubscriberDto GetMailSubscriberById(int id)
        {
            var subscriber = mapper.Map<MailSubscriberDto>(subscriberRepository.Get(id));
            subscriber.MailSubscriptionDto = mapper.Map<MailSubscriptionDto>(subscriptionRepository.Get(subscriber.MailSubscriptionId));

            return subscriber;
        }

        public MailSubscriberDto CreateMailSubscriber(MailSubscriberDto mailSubscriberDto)
        {
            var subscriber = mapper.Map<MailSubscriber>(mailSubscriberDto);

            return mapper.Map<MailSubscriberDto>(subscriberRepository.Create(subscriber));
        }

        public MailSubscriberDto UpdateMailSubscriber(MailSubscriberDto mailSubscriberDto)
        {
            var subscriber = mapper.Map<MailSubscriber>(mailSubscriberDto);

            return mapper.Map<MailSubscriberDto>(subscriberRepository.Update(subscriber));
        }

        public MailSubscriberDto DeleteMailSubsriber(int id)
        {
            return mapper.Map<MailSubscriberDto>(subscriberRepository.Delete(id));
        }

        public IEnumerable<MailSubscriptionDto> GetSubscriptionsBySubscribersEmail(int page, string sort, int limit)
        {
            // Idea is to get all subscription by email since subscriber in fact is anonimoous user
            throw new NotImplementedException();
        }
    }
}
