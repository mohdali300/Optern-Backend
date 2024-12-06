using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class VInterviewQuestionsConfiguration : IEntityTypeConfiguration<VInterviewQuestions>
    {
        public void Configure(EntityTypeBuilder<VInterviewQuestions> builder)
        {
            // Table Name
            builder.ToTable("VInterviewQuestions");


               builder.HasKey(x => x.Id);   

            builder.Property(vq => vq.VInterviewID)
                .IsRequired();

            builder.Property(vq => vq.VQuestionID)
                .IsRequired();

            // Relationships
            builder.HasOne(vq => vq.VInterview)
                .WithMany(v => v.VInterviewQuestions)
                .HasForeignKey(vq => vq.VInterviewID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(vq => vq.VQuestions)
                .WithMany(q => q.VInterviewQuestions)
                .HasForeignKey(vq => vq.VQuestionID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
