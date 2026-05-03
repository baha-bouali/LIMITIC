using LIMTIC.Domain.Entities.Publications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Publications
{
    public class BookChapterConfiguration : IEntityTypeConfiguration<BookChapterEntity>
    {
        public void Configure(EntityTypeBuilder<BookChapterEntity> builder)
        {
            builder.HasKey(b => b.PublicationId);

            builder.Property(b => b.BookTitle)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(b => b.Publisher)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(b => b.Isbn)
                   .HasMaxLength(20);

            builder.Property(b => b.Pages)
                   .HasMaxLength(50);

            builder.HasOne(b => b.Publication)
                   .WithOne(p => p.BookChapter)
                   .HasForeignKey<BookChapterEntity>(b => b.PublicationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}