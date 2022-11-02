using CoreWebApi.Models.Account;
using CoreWebApi.Models.Tenant;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreWebApi.Data
{
    public class SeerDbContext : IdentityDbContext
    {
        public SeerDbContext(DbContextOptions<SeerDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        public DbSet<Tenant> Tenants { get; set; }
    }
}
