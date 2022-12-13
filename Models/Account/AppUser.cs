using Microsoft.AspNetCore.Identity;

namespace CoreWebApi.Models.Account
{
    public class AppUser : IdentityUser
    {
        public string AvatarUrl { get; set; }

        public AppUser() : base()
        {
        }
    }
}
