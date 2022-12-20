using Microsoft.AspNetCore.Identity;

namespace CoreWebApi.Models.Account
{
    public class ApplicationUser : IdentityUser
    {
        public string AvatarUrl { get; set; }

        //public ApplicationUser() : base()
        //{
        //}
    }
}
