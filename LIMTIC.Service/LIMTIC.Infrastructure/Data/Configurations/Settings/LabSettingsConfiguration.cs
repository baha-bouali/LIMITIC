using LIMTIC.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Settings
{
    public class LabSettingsConfiguration : IEntityTypeConfiguration<LabSettings>
    {
        public void Configure(EntityTypeBuilder<LabSettings> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.LabName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.LabSlogan).HasMaxLength(250);
            builder.Property(e => e.ContactEmail).HasMaxLength(200);
            builder.Property(e => e.Address).HasMaxLength(500);
            builder.Property(e => e.Phone).HasMaxLength(50);
            builder.Property(e => e.LogoUrl).HasMaxLength(1000);

            builder.Property(e => e.SmtpHost).HasMaxLength(500);
            builder.Property(e => e.SmtpPort).IsRequired();
            builder.Property(e => e.SmtpUsername).HasMaxLength(500);
            builder.Property(e => e.SmtpPasswordHash).HasMaxLength(1000);
            builder.Property(e => e.SmtpUseTls).IsRequired();

            builder.ToTable("LabSettings");

            // Seed a single row with a fixed ID so GET never returns null
            var defaultSettings = LabSettings.CreateDefault();
            defaultSettings.Id = Guid.Parse("00000000-0000-0000-0000-000000000001");
            defaultSettings.CreatedAtUtc = DateTime.UtcNow;
            defaultSettings.CreatedBy = Guid.Empty;

            builder.HasData(defaultSettings);
        }
    }
}
