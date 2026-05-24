using LIMTIC.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Events
{
    public class SpeakerConfiguration : IEntityTypeConfiguration<SpeakerEntity>
    {
        public void Configure(EntityTypeBuilder<SpeakerEntity> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.Email)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(s => s.Institution)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(s => s.Role)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.Subject)
                   .HasMaxLength(2000);

            builder.HasOne(s => s.Event)
                   .WithMany(e => e.Speakers)
                   .HasForeignKey(s => s.EventId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}