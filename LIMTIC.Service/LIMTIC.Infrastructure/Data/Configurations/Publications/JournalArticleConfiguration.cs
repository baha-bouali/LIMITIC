using LIMTIC.Domain.Entities.Publications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Publications
{
    public class JournalArticleConfiguration : IEntityTypeConfiguration<JournalArticleEntity>
    {
        public void Configure(EntityTypeBuilder<JournalArticleEntity> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.JournalName)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(a => a.Volume)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.Number)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.Pages)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.Ranking)
                   .HasConversion<string>()
                   .IsRequired();

            builder.HasOne(a => a.Publication)
                   .WithOne(p => p.JournalArticle)
                   .HasForeignKey<JournalArticleEntity>(a => a.Id)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}