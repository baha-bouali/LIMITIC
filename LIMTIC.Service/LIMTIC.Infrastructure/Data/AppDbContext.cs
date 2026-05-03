using LIMTIC.Domain.Entities;
using LIMTIC.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ResetPassword> ResetPasswords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ResetPasswordConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
