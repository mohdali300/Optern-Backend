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
    public class PTPUsersConfiguration : IEntityTypeConfiguration<PTPUsers>
    {
        public void Configure(EntityTypeBuilder<PTPUsers> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(ptp => ptp.User)
                   .WithMany(u => u.PeerToPeerInterviewUsers) 
                   .HasForeignKey(ptp => ptp.UserID) 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ptp => ptp.PeerToPeerInterview)
                   .WithMany(ptpInt => ptpInt.PeerToPeerInterviewUsers) 
                   .HasForeignKey(ptp => ptp.PTPIId) 
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
