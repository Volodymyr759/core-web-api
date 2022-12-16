using System.Collections.Generic;
using System.Security.Claims;

namespace CoreWebApi.Services.AccountService
{
    public interface ITokenService
    {
        bool IsTokenExpired(string token);
        string GenerateAccessToken(IEnumerable<Claim> claims, double period);
        string GenerateRandomToken();
        string GetUserEmailFromExpiredToken(string token);
    }
}
