using AutoMapper;
using CoreWebApi.Data;

namespace CoreWebApi.Services
{
    public abstract class BaseService<T> where T : class
    {
        public IMapper mapper { get; }

        public IRepository<T> repository { get; }

        public BaseService(IMapper mapper, IRepository<T> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }
    }
}
