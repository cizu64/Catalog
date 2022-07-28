using Catalog.Auth.Model;
using EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Auth.Infrastructure
{
    public class AuthContext : DbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
    }
}
