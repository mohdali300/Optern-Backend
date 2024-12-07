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
            #region Attributes

            // Table Name

            builder.ToTable("PTPUsers");

            // Primary Key

            builder.HasKey(ptp => ptp.Id);
            builder.Property(ptp => ptp.Id)
                   .ValueGeneratedOnAdd();

            // Properties

            builder.Property(ptp => ptp.UserID)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(ptp => ptp.PTPIId)
                   .IsRequired();

            // Indexes


            builder.HasIndex(x => x.PTPIId)
                   .HasDatabaseName("IX_PTPUsers_PTPIId");

            builder.HasIndex(x => x.UserID)
                   .HasDatabaseName("IX_PTPUsers_UserID");
            #endregion

            #region Relations
            builder.HasOne(ptp => ptp.User)
                   .WithMany(u => u.PeerToPeerInterviewUsers) 
                   .HasForeignKey(ptp => ptp.UserID) 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ptp => ptp.PeerToPeerInterview)
                   .WithMany(ptpInt => ptpInt.PeerToPeerInterviewUsers) 
                   .HasForeignKey(ptp => ptp.PTPIId) 
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
