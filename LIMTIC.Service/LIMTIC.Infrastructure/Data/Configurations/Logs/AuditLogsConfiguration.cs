using LIMTIC.Domain.Entities.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Logs
{
    public class AuditLogsConfiguration : IEntityTypeConfiguration<AuditLogsEntity>
    {
        public void Configure(EntityTypeBuilder<AuditLogsEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ActorId)
                .IsRequired();

            builder.HasOne(e => e.Actor)
                .WithMany()
                .HasForeignKey(e => e.ActorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(e => e.Action)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(e => e.Resource)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(e => e.Timestamp)
                .IsRequired();
        }
    }
}
