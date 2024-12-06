using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class PTPInterviewConfiguration : IEntityTypeConfiguration<PTPInterview>
    {
        public void Configure(EntityTypeBuilder<PTPInterview> builder)
        {
            // Table Name
            builder.ToTable("PTPInterviews");

            builder.HasKey(i => i.Id);

            // Properties
            builder.Property(i => i.Id)
                .IsRequired();

            builder.Property(i => i.Category)
                .IsRequired();

            builder.Property(i => i.ScheduledTime)
                .IsRequired();

            builder.Property(i => i.Status)
                .IsRequired();

            builder.Property(i => i.Duration)
                .IsRequired();

            // Relationships

            builder.HasMany(i => i.PeerToPeerInterviewUsers)
                .WithOne()
                .HasForeignKey(u => u.PeerToPeerInterview)
                .OnDelete(DeleteBehavior.Cascade);  

            builder.HasMany(i => i.PTPQuestionInterviews)
                .WithOne(q => q.PTPInterview)
                .HasForeignKey(q => q.PTPInterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.PTPFeedBacks)
                .WithOne()
                .HasForeignKey(f => f.PTPInterviewId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
