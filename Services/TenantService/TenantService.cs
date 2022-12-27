using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CoreWebApi.Services.TenantService
{
    public class TenantService : ITenantService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Tenant> repository;

        public TenantService(IMapper mapper, IRepository<Tenant> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public IEnumerable<TenantDto> GetAllTenants(int limit, int page, string search, string sort_field, string sort)
        {
            // search by FirstName or LastName
            Expression<Func<Tenant, bool>> searchQuery = null;
            if (search.Trim().Length > 0) searchQuery = t => t.FirstName.Contains(search) || t.LastName.Contains(search);

            // sorting - newest first
            Func<IQueryable<Tenant>, IOrderedQueryable<Tenant>> orderBy = null;
            orderBy = sort == "asc" ? q => q.OrderBy(s => s.Id) : orderBy = q => q.OrderByDescending(s => s.Id);

            return mapper.Map<IEnumerable<TenantDto>>(repository.GetAll(limit, page, searchQuery, orderBy));
        }

        public TenantDto GetTenantById(int id)
        {
            return mapper.Map<TenantDto>(repository.Get(t => t.Id == id));
        }

        public TenantDto CreateTenant(CreateTenantDto createTenantDto)
        {
            var tenant = mapper.Map<Tenant>(createTenantDto);

            return mapper.Map<TenantDto>(repository.Create(tenant));
        }

        public void DeleteTenant(TenantDto tenantDto)
        {
            repository.Delete(mapper.Map<Tenant>(tenantDto));
        }

        public TenantDto UpdateTenant(TenantDto tenantDto)
        {
            var tenant = mapper.Map<Tenant>(tenantDto);

            return mapper.Map<TenantDto>(repository.Update(tenant));
        }
    }
}
