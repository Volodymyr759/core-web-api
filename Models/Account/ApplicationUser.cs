using Microsoft.AspNetCore.Identity;
using System;

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
