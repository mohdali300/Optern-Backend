using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class PTPQuestionInterviewConfiguration : IEntityTypeConfiguration<PTPQuestionInterview>
    {
        public void Configure(EntityTypeBuilder<PTPQuestionInterview> builder)
        {
            #region Attributes

            // Table Name

            builder.ToTable("PTPQuestionInterviews");
            // Primary Key

            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id)
                   .ValueGeneratedOnAdd();
            // Properties

            builder.Property(q => q.PTPQuestionId)
                   .IsRequired();

            builder.Property(q => q.PTPInterviewId)
                   .IsRequired();

            // Indexes

            builder.HasIndex(q => q.PTPQuestionId)
                   .HasDatabaseName("IX_PTPQuestionInterview_PTPQuestionId");

            builder.HasIndex(q => q.PTPInterviewId)
                   .HasDatabaseName("IX_PTPQuestionInterview_PTPInterviewId");
            #endregion

            #region Relations
            builder.HasOne(q => q.PTPQuestion)
                .WithMany(p => p.PTPQuestionInterviews)
                .HasForeignKey(q => q.PTPQuestionId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(q => q.PTPInterview)
                .WithMany(i => i.PTPQuestionInterviews)
                .HasForeignKey(q => q.PTPInterviewId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
