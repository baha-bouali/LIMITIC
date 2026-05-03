using LIMTIC.Domain.Entities.Publications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Publications
{
    public class NationalConferenceConfiguration : IEntityTypeConfiguration<NationalConferenceEntity>
    {
        public void Configure(EntityTypeBuilder<NationalConferenceEntity> builder)
        {
            builder.HasKey(c => c.PublicationId);

            builder.Property(c => c.ConferenceName)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(c => c.Location)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(c => c.Pages)
                   .HasMaxLength(50);

            builder.HasOne(c => c.Publication)
                   .WithOne(p => p.NationalConference)
                   .HasForeignKey<NationalConferenceEntity>(c => c.PublicationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}