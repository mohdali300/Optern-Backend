using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class VInterviewConfiguration : IEntityTypeConfiguration<VInterview>
    {
        public void Configure(EntityTypeBuilder<VInterview> builder)
        {
            builder.ToTable("VInterviews");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Category)
                .IsRequired();

            builder.Property(v => v.ScheduledTime)
                .IsRequired();

            // Relationships

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
        }
    }
}
