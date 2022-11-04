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

        public IEnumerable<TenantDto> GetAllTenants(int limit, int page, string search, string sort_field, string sort)
        {
            // search by FirstName or LastName
            Expression<Func<Tenant, bool>> searchQuery = null;
            if (search.Trim().Length > 0) searchQuery = t => t.FirstName.Contains(search) || t.LastName.Contains(search);

            // sorting - newest first
            Func<IQueryable<Tenant>, IOrderedQueryable<Tenant>> orderBy = null;
            if (sort == "asc")
            {
                orderBy = q => q.OrderBy(s => s.Id);
            }
            else
            {
                orderBy = q => q.OrderByDescending(s => s.Id);
            }

            return _mapper.Map<IEnumerable<TenantDto>>(_repository.GetAll(limit, page, searchQuery, orderBy));
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
