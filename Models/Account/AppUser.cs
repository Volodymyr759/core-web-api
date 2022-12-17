using Microsoft.AspNetCore.Identity;
using System;

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
