using CoreWebApi.Library.SearchResult;
using CoreWebApi.Models.Account;
using System.Collections.Generic;

namespace CoreWebApi.Services
{
    public interface IAccountService
    {
        SearchResult<ApplicationUserDto> GetUsersSearchResultAsync(int limit, int page, string search, IEnumerable<ApplicationUser> users);
    }
}
