using System.Collections.Generic;

namespace CoreWebApi.Models.Account
{
    public class AuthModel
    {
        public string Email { get; set; }

        public IList<string> Roles { get; set; }

        public TokenModel Tokens { get; set; }
    }
}
