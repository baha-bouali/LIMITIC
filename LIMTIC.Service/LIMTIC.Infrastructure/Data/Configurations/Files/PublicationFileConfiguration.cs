using LIMTIC.Domain.Entities.Files;
using LIMTIC.Domain.Entities.Publications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Files
{
    public class PublicationFileConfiguration : IEntityTypeConfiguration<PublicationFileEntity>
    {
        public void Configure(EntityTypeBuilder<PublicationFileEntity> builder)
        {
            builder.ToTable("PublicationFiles");

            // Primary key
            builder.HasKey(x => x.Id);

            // Required fields
            builder.Property(x => x.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.BlobFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.BlobPath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.SizeInBytes)
                .IsRequired();

            builder.Property(x => x.UploadedAtUtc)
                .IsRequired();

            builder.HasOne(x => x.Publication)
                .WithMany(p => p.Files)
                .HasForeignKey(x => x.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.PublicationId);

            builder.HasIndex(x => x.BlobPath)
                .IsUnique();
        }
    }
}
