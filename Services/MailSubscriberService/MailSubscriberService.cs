using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services.MailSubscriptionService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWebApi.Services.MailSubscriberService
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
            var mappedSubscriberDtos = mapper.Map<IEnumerable<MailSubscriberDto>>(subscriberRepository.GetAll(limit, page, null, orderBy));

            if (((List<MailSubscriberDto>)mappedSubscriberDtos).Count > 0)
            {
                foreach (var ms in mappedSubscriberDtos)
                    ms.MailSubscriptionDto = mapper.Map<MailSubscriptionDto>(subscriptionRepository.Get(ms.MailSubscriptionId));
            }

            return mappedSubscriberDtos;
        }

        public MailSubscriberDto GetMailSubscriberById(int id)
        {
            throw new System.NotImplementedException();
        }

        public MailSubscriberDto Subscribe(MailSubscriberDto mailSubscriberDto)
        {
            throw new System.NotImplementedException();
        }

        public MailSubscriberDto Unsubscribe(int id, bool isSubscribed)
        {
            throw new System.NotImplementedException();
        }

        public MailSubscriberDto DeleteMailSubsriber(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
