using CoreWebApi.Models.Account;
using CoreWebApi.Models.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoreWebApi.Data
{
    public class SeerDbContext : IdentityDbContext<ApplicationUser>
    {
        public SeerDbContext(DbContextOptions<SeerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
    }
}
