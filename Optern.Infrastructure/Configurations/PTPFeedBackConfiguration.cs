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

            #region Attributes 

            // Table Name

            builder.ToTable("PTPFeedBack");

            // Primary Key

            builder.HasKey(fb => fb.Id);
            builder.Property(fb => fb.Id)
                   .ValueGeneratedOnAdd();

             // Properties
            builder.Property(fb => fb.InterviewerPerformance)
                   .IsRequired();

            builder.Property(fb => fb.IntervieweePerformance)
                   .IsRequired();

            // Indexes

            builder.HasIndex(fb => fb.PTPInterviewId)
              .HasDatabaseName("IX_PTPFeedBack_PTPInterviewId");

            #endregion

            #region Relations
            builder.HasOne(fb => fb.PTPInterview)
                   .WithMany(ptp => ptp.PTPFeedBacks)
                   .HasForeignKey(fb => fb.PTPInterviewId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion


        }
    }

}
