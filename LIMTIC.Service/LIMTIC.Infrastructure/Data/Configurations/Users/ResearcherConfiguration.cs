using LIMTIC.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Users
{
    public class ResearcherConfiguration : IEntityTypeConfiguration<ResearcherEntity>
    {
        public void Configure(EntityTypeBuilder<ResearcherEntity> builder)
        {
            builder.HasKey(e => e.UserId);

            builder
                .HasOne(e => e.User)
                .WithOne(e => e.Researcher)
                .HasForeignKey<ResearcherEntity>(e => e.UserId);
        }
    }
}
