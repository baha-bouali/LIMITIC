using LIMTIC.Domain.Entities.ResearchAxis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.ResearchAxis
{
    public class ResearchAxisConfiguration : IEntityTypeConfiguration<ResearchAxisEntity>
    {
        public void Configure(EntityTypeBuilder<ResearchAxisEntity> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(a => a.Description)
                   .IsRequired();

            builder.Property(a => a.Themes)
                   .HasConversion(
                       v => string.Join(',', v),
                       v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToArray())
                   .Metadata.SetValueComparer(
                       new ValueComparer<string[]>(
                           (a, b) => a.SequenceEqual(b),
                           c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                           c => c.ToArray()));

            builder.Property(a => a.Color)
                   .HasMaxLength(50)
                   .IsRequired(false);

            builder.HasOne(a => a.Responsible)
                   .WithMany()
                   .HasForeignKey(a => a.ResponsibleId)
                   .OnDelete(DeleteBehavior.SetNull)
                   .IsRequired(false);
        }
    }
}