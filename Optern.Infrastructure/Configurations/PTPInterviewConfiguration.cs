using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class PTPInterviewConfiguration : IEntityTypeConfiguration<PTPInterview>
    {
        public void Configure(EntityTypeBuilder<PTPInterview> builder)
        {

            #region Attributes

            // Table Name

            builder.ToTable("PTPInterviews");

            // Primary Key

            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                   .ValueGeneratedOnAdd();

            // Properties

            builder.Property(i => i.Category)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.Property(i => i.ScheduledDate)
            .IsRequired();

            builder.Property(i => i.ScheduledTime)
                .HasConversion<int>();

            builder.Property(i => i.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);

            #endregion

            #region Relationships

            builder.HasMany(i => i.PeerToPeerInterviewUsers)
                .WithOne(ptp => ptp.PeerToPeerInterview) 
                .HasForeignKey(ptp => ptp.PTPIId)       
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(i => i.PTPQuestionInterviews)
                .WithOne(q => q.PTPInterview)
                .HasForeignKey(q => q.PTPInterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.PTPFeedBacks)
                .WithOne(i=>i.PTPInterview)
                .HasForeignKey(f => f.PTPInterviewId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
