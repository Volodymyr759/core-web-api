using CoreWebApi.Models.Account;
using System.Collections.Generic;

namespace CoreWebApi.Services
{
    public class AuthModel
    {
        public ApplicationUserDto User { get; set; }

        public IList<string> Roles { get; set; }

        public TokenModel Tokens { get; set; }
    }
}
