using AutoMapper;
using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreWebApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMapper mapper;

        public AccountService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public SearchResult<ApplicationUserDto> GetUsersSearchResultAsync(int limit, int page, string search, IEnumerable<ApplicationUser> users)
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

    }
}
