using LIMTIC.Domain.Entities.Publications;
using LIMTIC.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Publications
{
    public class PublicationConfiguration : IEntityTypeConfiguration<PublicationEntity>
    {
        public void Configure(EntityTypeBuilder<PublicationEntity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Abstract)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.Visibility)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.Keywords)
                .HasConversion(
                    v => string.Join(',', v ?? Array.Empty<string>()),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Metadata.SetValueComparer(
                    new ValueComparer<string[]>(
                        (a, b) => a.SequenceEqual(b),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToArray()));

            builder.Property(p => p.AttachedPdfs)
                .HasConversion(
                    v => string.Join(',', v ?? Array.Empty<string>()),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Metadata.SetValueComparer(
                    new ValueComparer<string[]>(
                        (a, b) => a.SequenceEqual(b),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToArray()));

            builder.Property(p => p.ResearchAxisId)
                .IsRequired();

            builder.HasOne(p => p.ResearchAxis)
                .WithMany(a => a.Publications)
                .HasForeignKey(p => p.ResearchAxisId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasDiscriminator(p => p.Type)
                .HasValue<JournalArticleEntity>(PublicationType.ArticleJournal)
                .HasValue<TechnicalReportEntity>(PublicationType.TechnicalReport)
                .HasValue<BookChapterEntity>(PublicationType.ChapterBook)
                .HasValue<NationalConferenceEntity>(PublicationType.ConferenceNational)
                .HasValue<InternationalConferenceEntity>(PublicationType.ConferenceInternational);
        }
    }
}