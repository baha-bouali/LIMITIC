using LIMTIC.Domain.Entities.Publications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations.Publications
{
    public class PublicationInternalAuthorConfiguration : IEntityTypeConfiguration<PublicationInternalAuthorEntity>
    {
        public void Configure(EntityTypeBuilder<PublicationInternalAuthorEntity> builder)
        {
            // Composite Primary Key
            builder.HasKey(pa => new { pa.PublicationId, pa.UserId });

            builder.HasOne(pa => pa.Publication)
                   .WithMany(p => p.InternalAuthors)
                   .HasForeignKey(pa => pa.PublicationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pa => pa.User)
                   .WithMany(u => u.CoAuthoredPublications) 
                   .HasForeignKey(pa => pa.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Set to Restrict if you don't want a user deletion to cascade
        }
    }
}