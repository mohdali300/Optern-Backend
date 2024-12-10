using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class VInterviewConfiguration : IEntityTypeConfiguration<VInterview>
    {
        public void Configure(EntityTypeBuilder<VInterview> builder)
        {
            #region Attributes

            //Table Name
            builder.ToTable("VInterview", t =>
            {
                t.HasCheckConstraint(
                    "CK_VInterviews_ScheduledTime_Future",
                    "\"ScheduledTime\" > NOW()");  
            });
            // Primary Key

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id)
              .ValueGeneratedOnAdd();

            // Properties

            builder.Property(v => v.Category)
                .IsRequired().HasConversion<string>();

            builder.Property(v => v.ScheduledTime)
               .IsRequired()                       
               .HasColumnType("timestamp")         
               .HasComment("ScheduledTime must be in the future and is required.");

            //Indexes 
            builder.HasIndex(v => v.ScheduledTime).HasDatabaseName("IX_VInterview_ScheduledTime");
            #endregion

            #region Relations
            builder.HasOne(v => v.User)
                .WithMany(u => u.JoinedVirtualInterviews)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.VirtualFeedBack)
                .WithOne()
                .HasForeignKey<VInterview>(v => v.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.VInterviewQuestions)
                .WithOne(vq => vq.VInterview)
                .HasForeignKey(vq => vq.VInterviewID)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
