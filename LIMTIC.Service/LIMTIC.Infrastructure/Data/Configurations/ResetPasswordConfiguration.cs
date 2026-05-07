using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMTIC.Domain.Entities.ResetPassword;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIMTIC.Infrastructure.Data.Configurations
{
    public class ResetPasswordConfiguration : IEntityTypeConfiguration<ResetPasswordEntity>
    {
        public void Configure(EntityTypeBuilder<ResetPasswordEntity> builder)
        {
            builder.HasKey(rp => rp.Id);

            builder.HasOne(rp => rp.User)
                   .WithMany()
                   .HasForeignKey(rp => rp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
