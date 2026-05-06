using LIMTIC.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Users
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder
                .Property(e => e.FirstName)
                .HasMaxLength(100);

            builder
                .Property(e => e.LastName)
                .HasMaxLength(100);

            // Unique index on Email for fast lookups and uniqueness enforcement
            builder.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            // Index on IsActive for filtering active users
            builder.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Users_IsActive");

            // Ensure strings are not empty (not just non-null)
            builder.ToTable(t => t.HasCheckConstraint("CK_User_FirstName_NotEmpty", "LTRIM(RTRIM(\"FirstName\")) <> ''"));
            builder.ToTable(t => t.HasCheckConstraint("CK_User_LastName_NotEmpty", "LTRIM(RTRIM(\"LastName\")) <> ''"));
        }
    }
}
