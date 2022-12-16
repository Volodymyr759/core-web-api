using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApi.Models.Account
{
    public class AuthModel
    {
        public string Email { get; set; }

        public IList<string> Roles { get; set; }

        public TokenModel Tokens { get; set; }
    }
}
