using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWebApi.Services.MailSubscriptionService
{
    public class MailSubscriptionService : IMailSubscriptionService
    {
        private readonly IMapper mapper;
        private readonly IRepository<MailSubscription> repository;

        public MailSubscriptionService(IMapper mapper, IRepository<MailSubscription> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public IEnumerable<MailSubscriptionDto> GetAllMailSubscriptions(int page, string sort, int limit)
        {
            // sorting only by Title
            Func<IQueryable<MailSubscription>, IOrderedQueryable<MailSubscription>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Title) : orderBy = q => q.OrderByDescending(s => s.Title);

            return mapper.Map<IEnumerable<MailSubscriptionDto>>(repository.GetAll(limit, page, null, orderBy));
        }

        public MailSubscriptionDto GetMailSubscriptionById(int id)
        {
            return mapper.Map<MailSubscriptionDto>(repository.Get(t => t.Id == id));
        }

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

        public MailSubscriptionDto DeleteMailSubscription(int id)
        {
            return mapper.Map<MailSubscriptionDto>(repository.Delete(id));
        }
    }
}
