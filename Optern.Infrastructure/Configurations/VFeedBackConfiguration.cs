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
            #region Attributes

            // Table Name
            builder.ToTable("VFeedBack", t =>
            {
                t.HasCheckConstraint(
                    "CK_VFeedBack_PerformanceScore",
                    "\"PerformanceScore\" BETWEEN 0 AND 100");
            });
            // Primary Key
            builder.HasKey(f => f.Id);
            builder.Property(ur => ur.Id)
           .ValueGeneratedOnAdd();

            // Properties

            builder.Property(f => f.PerformanceScore)
              .IsRequired();

            builder.Property(f => f.Strengths)
                   .HasMaxLength(500);

            builder.Property(f => f.Weaknesses)
                   .HasMaxLength(500);

            builder.Property(f => f.Recommendations)
                   .HasMaxLength(1000);

            builder.Property(f => f.VInterviewID)
                   .IsRequired();

            builder.Property(f => f.PerformanceScore)
         .IsRequired()
         .HasColumnType("integer")
         .HasComment("Performance score must be between 0 and 100 inclusive.");

            #endregion

            #region Relations
            builder.HasOne(f => f.VirtualInterview)
                   .WithOne(v => v.VirtualFeedBack)
                   .HasForeignKey<VFeedBack>(f => f.VInterviewID)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion

        }
    }

}
