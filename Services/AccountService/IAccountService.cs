using CoreWebApi.Library;
using CoreWebApi.Models.Account;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreWebApi.Services
{
    public interface IAccountService
    {
        ISearchResult<ApplicationUserDto> GetUsersSearchResultAsync(int limit, int page, string search, IEnumerable<ApplicationUser> users);
        ApplicationUserDto GetApplicationUserDto(ApplicationUser user);
        Task<ApplicationUserDto> PartialUpdateAsync(ApplicationUser user, JsonPatchDocument<object> patchDocument);
        Task<ApplicationUserDto> PartialUpdateAsync(ApplicationUser user);
    }
}
