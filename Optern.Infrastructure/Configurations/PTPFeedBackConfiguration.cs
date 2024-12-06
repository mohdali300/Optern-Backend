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
    public class PTPFeedBackConfiguration : IEntityTypeConfiguration<PTPFeedBack>
    {
        public void Configure(EntityTypeBuilder<PTPFeedBack> builder)
        {
            builder.HasOne(fb => fb.PTPInterview)
                   .WithMany(ptp => ptp.PTPFeedBacks)
                   .HasForeignKey(fb => fb.PTPInterviewId)
                   .OnDelete(DeleteBehavior.Cascade);  

           
        }
    }

}
