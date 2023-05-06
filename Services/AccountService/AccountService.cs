using AutoMapper;
using CoreWebApi.Data;
using CoreWebApi.Library;
using CoreWebApi.Models.Account;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMapper mapper;
        private readonly IRepository<ApplicationUser> repository;

        public AccountService(IMapper mapper, IRepository<ApplicationUser> repository)
        {
            this.mapper = mapper;
            this.repository = repository;
        }

        public ApplicationUserDto GetApplicationUserDto(ApplicationUser user) => mapper.Map<ApplicationUserDto>(user);

        public ISearchResult<ApplicationUserDto> GetUsersSearchResultAsync(int limit, int page, string search, IEnumerable<ApplicationUser> users)
        {
            if (!string.IsNullOrEmpty(search)) users = users.Where(u => u.UserName.Contains(search)).ToList();

            // Sorting by Email for now, because UserName can be changed easier by user
            users = users.OrderBy(u => u.Email);

            return new SearchResult<ApplicationUserDto>
            {
                CurrentPageNumber = page,
                Order = null,
                PageSize = limit,
                PageCount = Convert.ToInt32(Math.Ceiling((double)users.Count() / limit)),
                SearchCriteria = string.Empty,
                TotalItemCount = users.Count(),
                ItemList = (List<ApplicationUserDto>)mapper.Map<IEnumerable<ApplicationUserDto>>(users.Skip((page - 1) * limit).Take(limit))
            };
        }

        public async Task<ApplicationUserDto> PartialUpdateAsync(ApplicationUser user, JsonPatchDocument<object> patchDocument)
        {
            patchDocument.ApplyTo(user);

            return mapper.Map<ApplicationUserDto>(await repository.SaveAsync(user));
        }

        public Task<ApplicationUserDto> PartialUpdateAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
