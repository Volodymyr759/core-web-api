using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreWebApi.Services.TenantService
{
    public class TenantService : ITenantService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Tenant> _repository;

        public TenantService(IMapper mapper, IRepository<Tenant> repository)
        {
            this._mapper = mapper;
            this._repository = repository;
        }

        public IEnumerable<TenantDto> GetAllTenants()
        {
            //var tenants = new List<ITenant>()
            //{
            //    new Tenant{ Id = 1, FirstName= "Tom", LastName="Foster", Email="tf@gmail.com", Phone="123123123"},
            //    new Tenant{ Id = 2, FirstName= "John", LastName="Done", Email="jd1@gmail.com", Phone="123123122"},
            //    new Tenant{ Id = 3, FirstName= "Jane", LastName="Dane", Email="jd2@gmail.com", Phone="123123121"},
            //};
            //Expression<Func<Tenant, bool>> searchQuery = t => t.FirstName.Contains("J");
            Expression<Func<Tenant, bool>> searchQuery = t => t.FirstName.Contains("e") || t.LastName.Contains("e");
            //Func<IQueryable<Tenant>, IOrderedQueryable<Tenant>> orderBy = q => q.OrderBy(s => s.FirstName);
            Func<IQueryable<Tenant>, IOrderedQueryable<Tenant>> orderBy;
            if (searchQuery != null)
            {
                orderBy = q => q.OrderBy(s => s.FirstName);
            }
            else
            {
                orderBy = q => q.OrderBy(s => s.LastName);
            }

            var tenantDtos = _repository.GetAll(searchQuery, orderBy);

            return _mapper.Map<IEnumerable<TenantDto>>(tenantDtos);
        }

        public TenantDto GetTenantById(int id)
        {
            return _mapper.Map<TenantDto>(_repository.Get(t => t.Id == id));
        }

        public TenantDto CreateTenant(CreateTenantDto createTenantDto)
        {
            var tenant = _mapper.Map<Tenant>(createTenantDto);

            return _mapper.Map<TenantDto>(_repository.Create(tenant));
        }

        public void DeleteTenant(TenantDto tenantDto)
        {
            _repository.Delete(_mapper.Map<Tenant>(tenantDto));
        }

        public TenantDto UpdateTenant(TenantDto tenantDto)
        {
            var tenant = _mapper.Map<Tenant>(tenantDto);

            return _mapper.Map<TenantDto>(_repository.Update(tenant));
        }
    }
}
