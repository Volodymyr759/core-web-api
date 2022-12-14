using CoreWebApi.Models.Account;
using CoreWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoreWebApi.Data
{
    public class SeerDbContext : IdentityDbContext<ApplicationUser>
    {
        public SeerDbContext(DbContextOptions<SeerDbContext> options)
            : base(options) { }

        public DbSet<Candidate> Candidates { get; set; }

        public DbSet<CompanyService> CompanyServices { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<MailSubscriber> MailSubscribers { get; set; }

        public DbSet<MailSubscription> MailSubscriptions { get; set; }

        public DbSet<Office> Offices { get; set; }

        public DbSet<Vacancy> Vacancies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "Users"); });
            modelBuilder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Roles"); });
            modelBuilder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
            modelBuilder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
        }
    }
}
