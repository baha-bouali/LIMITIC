using LIMTIC.Domain.Entities.Publications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Publications
{
    public class InternationalConferenceConfiguration : IEntityTypeConfiguration<InternationalConferenceEntity>
    {
        public void Configure(EntityTypeBuilder<InternationalConferenceEntity> builder)
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

            builder.Property(c => c.Ranking)
                   .HasConversion<string>()
                   .IsRequired();

            builder.HasOne(c => c.Publication)
                   .WithOne(p => p.InternationalConference)
                   .HasForeignKey<InternationalConferenceEntity>(c => c.PublicationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}