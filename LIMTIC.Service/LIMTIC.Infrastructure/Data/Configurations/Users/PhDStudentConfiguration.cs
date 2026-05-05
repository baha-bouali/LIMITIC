using LIMTIC.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Users
{
    public class PhDStudentConfiguration : IEntityTypeConfiguration<PhDStudentEntity>
    {
        public void Configure(EntityTypeBuilder<PhDStudentEntity> builder)
        {
            builder.HasKey(e => e.UserId);

            builder
                .HasOne(e => e.User)
                .WithOne(e => e.PhDStudent)
                .HasForeignKey<PhDStudentEntity>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Supervisor)
                .WithMany()
                .HasForeignKey(e => e.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
