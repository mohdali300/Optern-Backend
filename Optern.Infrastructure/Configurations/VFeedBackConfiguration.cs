using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Configurations
{
    public class VFeedBackConfiguration : IEntityTypeConfiguration<VFeedBack>
    {
        public void Configure(EntityTypeBuilder<VFeedBack> builder)
        {
            builder.HasOne(f => f.VirtualInterview)
                   .WithOne(v => v.VirtualFeedBack)
                   .HasForeignKey<VFeedBack>(f => f.VInterviewID)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }

}
