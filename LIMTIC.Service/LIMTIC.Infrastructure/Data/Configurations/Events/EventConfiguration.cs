using LIMTIC.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Events
{
    public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
    {
        public void Configure(EntityTypeBuilder<EventEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(e => e.Type)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(e => e.StartDate)
                   .IsRequired();

            builder.Property(e => e.EndDate)
                   .IsRequired();

            builder.Property(e => e.Location)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(4000);

            builder.Property(e => e.Program)
                   .HasMaxLength(4000);

            builder.Property(e => e.PhotoFileNames)
                   .HasConversion(
                       v => string.Join(',', v),
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                   .Metadata.SetValueComparer(
                       new ValueComparer<List<string>>(
                           (a, b) => a.SequenceEqual(b),
                           c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                           c => c.ToList()));

            builder.Ignore(e => e.Status);

            builder.HasOne(e => e.ResearchAxis)
                   .WithMany()
                   .HasForeignKey(e => e.ResearchAxisId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Speakers)
                   .WithOne(s => s.Event)
                   .HasForeignKey(s => s.EventId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}