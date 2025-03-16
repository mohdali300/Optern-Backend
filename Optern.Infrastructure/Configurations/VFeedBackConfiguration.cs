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
            builder.ToTable("VFeedBack");

            // Primary Key
            builder.HasKey(f => f.Id);
            builder.Property(ur => ur.Id)
           .ValueGeneratedOnAdd();

            // Properties


            builder.Property(f => f.VirtualInterviewId)
                   .IsRequired();

            builder.Property(f => f.PerformanceScore)
         .IsRequired()
         .HasColumnType("integer")
         .HasComment("Performance score must be between 0 and 100 inclusive.");

            #endregion

            #region Relations
            builder.HasOne(f => f.VirtualInterview)
                   .WithOne(v => v.VirtualFeedBack)
                   .HasForeignKey<VFeedBack>(f => f.VirtualInterviewId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion

        }
    }

}
