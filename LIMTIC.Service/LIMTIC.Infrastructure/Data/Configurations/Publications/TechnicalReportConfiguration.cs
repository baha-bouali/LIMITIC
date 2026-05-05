using LIMTIC.Domain.Entities.Publications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Publications
{
    public class TechnicalReportConfiguration : IEntityTypeConfiguration<TechnicalReportEntity>
    {
        public void Configure(EntityTypeBuilder<TechnicalReportEntity> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReportNumber)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.Institution)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.HasOne(r => r.Publication)
                   .WithOne(p => p.TechnicalReport)
                   .HasForeignKey<TechnicalReportEntity>(r => r.Id)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}