using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CoreWebApi.Services.AccountService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims, double period)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]));

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: configuration["Tokens:Issuer"],
                audience: configuration["Tokens:Issuer"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(period),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            ));
        }

        public string GenerateRandomToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string GetUserEmailFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"])),
                    ValidateLifetime = false //here we don't care about the token's expiration date
                },
                out SecurityToken securityToken);
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return jwtSecurityToken.Payload.Sub;
        }

        public bool IsTokenExpired(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"])),
                    ValidateLifetime = true
                },
                out SecurityToken securityToken);
            return securityToken.ValidTo < DateTime.UtcNow;
        }
    }
}
