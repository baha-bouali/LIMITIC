using LIMTIC.Application.Abstractions;
using LIMTIC.Domain.Entities;
using LIMTIC.Domain.Shared;
using LIMTIC.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace LIMTIC.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService) : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ResetPassword> ResetPasswords { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _currentUserService.UserId;

            var addedEntries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in addedEntries)
            {
                if (entry.Entity.Id == Guid.Empty)
                    entry.Entity.Id = Guid.NewGuid();

                entry.Entity.CreatedBy = currentUserId;
                entry.Entity.CreatedAtUtc = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ResetPasswordConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
