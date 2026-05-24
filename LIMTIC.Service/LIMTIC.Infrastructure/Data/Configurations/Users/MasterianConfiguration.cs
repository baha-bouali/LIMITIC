using LIMTIC.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Users
{
    public class MasterianConfiguration : IEntityTypeConfiguration<MasterianEntity>
    {
        public void Configure(EntityTypeBuilder<MasterianEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder
                .HasOne(e => e.User)
                .WithOne(e => e.Masterian)
                .HasForeignKey<MasterianEntity>(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Supervisor)
                .WithMany()
                .HasForeignKey(e => e.SupervisorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
