using System.Collections.Generic;
using System.Security.Claims;

namespace CoreWebApi.Services.AccountService
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        string GetUserEmailFromExpiredToken(string token);
    }
}
