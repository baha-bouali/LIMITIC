using LIMTIC.Application.Abstractions;
using LIMTIC.Domain.Entities.Events;
using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Entities.RefreshToken;
using LIMTIC.Domain.Entities.ResearchAxis;
using LIMTIC.Domain.Entities.ResetPassword;
using LIMTIC.Domain.Entities.Users;
using LIMTIC.Domain.Shared;
using LIMTIC.Infrastructure.Data.Configurations.Events;
using LIMTIC.Infrastructure.Data.Configurations.Publications;
using LIMTIC.Infrastructure.Data.Configurations.RefreshToken;
using LIMTIC.Infrastructure.Data.Configurations.ResearchAxis;
using LIMTIC.Infrastructure.Data.Configurations.ResetPassword;
using LIMTIC.Infrastructure.Data.Configurations.Users;
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

        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<ResearcherEntity> Researchers { get; set; }
        public DbSet<PhDStudentEntity> PhDStudents { get; set; }
        public DbSet<MasterianEntity> Masterians { get; set; }

        public DbSet<ResearchAxisEntity> ResearchAxes { get; set; }

        public DbSet<EventEntity> Events { get; set; }
        public DbSet<SpeakerEntity> Speakers { get; set; }

        public DbSet<PublicationEntity> Publications { get; set; }
        public DbSet<ResetPasswordEntity> ResetPasswords { get; set; }
        public DbSet<PublicationInternalAuthorEntity> PublicationInternalAuthors { get; set; }

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
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PhDStudentConfiguration());
            modelBuilder.ApplyConfiguration(new MasterianConfiguration());
            modelBuilder.ApplyConfiguration(new ResearcherConfiguration());

            modelBuilder.ApplyConfiguration(new ResearchAxisConfiguration());

            modelBuilder.ApplyConfiguration(new EventConfiguration());
            modelBuilder.ApplyConfiguration(new SpeakerConfiguration());


            modelBuilder.ApplyConfiguration(new PublicationConfiguration());
            modelBuilder.ApplyConfiguration(new PublicationInternalAuthorConfiguration());

            modelBuilder.ApplyConfiguration(new ResetPasswordConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
