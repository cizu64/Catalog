using Catalog.Auth.Infrastructure.EntityConfigurations;
using Catalog.Auth.Model;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Auth.Infrastructure
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<IntegrationEvent> IntegrationEvent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new IntegrationEventEntityTypeConfiguration());
        }
    }
}
