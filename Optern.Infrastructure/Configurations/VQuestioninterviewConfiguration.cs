using Microsoft.EntityFrameworkCore;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class VQuestioninterviewConfiguration : IEntityTypeConfiguration<VQuestionInterview>
    {
        public void Configure(EntityTypeBuilder<VQuestionInterview> builder)
        {
            #region Attributes

            // Table Name

            builder.ToTable("VQuestionInterviews");
            // Primary Key

            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id)
                   .ValueGeneratedOnAdd();
            // Properties

            builder.Property(q => q.PTPQuestionId)
                   .IsRequired();



            // Indexes

            builder.HasIndex(q => q.PTPQuestionId)
                   .HasDatabaseName("IX_VQuestionInterview_PTPQuestionId");

            #endregion

            #region Relations
            builder.HasOne(q => q.PTPQuestion)
                .WithMany(p => p.VQuestionInterviews)
                .HasForeignKey(q => q.PTPQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder.HasOne(q => q.VInterview)
                .WithMany(i => i.VQuestionInterviews)
                .HasForeignKey(q => q.VInterviewId)
            .OnDelete(DeleteBehavior.Cascade);

            
            #endregion
        }
    }
}
