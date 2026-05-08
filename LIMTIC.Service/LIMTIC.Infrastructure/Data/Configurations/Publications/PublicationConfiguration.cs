using LIMTIC.Domain.Entities.Publications;
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
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .Metadata.SetValueComparer(
                    new ValueComparer<List<string>>(
                        (a, b) => a != null && b != null && a.SequenceEqual(b),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            builder.Property(p => p.AttachedPdfs)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .Metadata.SetValueComparer(
                    new ValueComparer<List<string>>(
                        (a, b) => a != null && b != null && a.SequenceEqual(b),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            builder.Property(p => p.ResearchAxisId)
                .IsRequired();

            builder.HasOne(p => p.ResearchAxis)
                .WithMany(a => a.Publications)
                .HasForeignKey(p => p.ResearchAxisId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- NEW AUTHORS CONFIGURATION ---

            // External Authors (JSON array mapped natively)
            builder.OwnsMany(p => p.ExternalAuthors, a =>
            {
                a.ToJson(); 
            });

            // Internal Authors mapping is handled primarily by PublicationInternalAuthorConfiguration
            // but we can explicitly set the navigation property mapping here just to be safe:
            builder.Navigation(p => p.InternalAuthors).AutoInclude(false);
        }
    }
}