using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class VQuestionsConfiguration : IEntityTypeConfiguration<VQuestions>
    {
        public void Configure(EntityTypeBuilder<VQuestions> builder)
        {
            // Table Name
            builder.ToTable("VQuestions");

            builder.HasKey(q => q.Id);

            // Properties


            builder.Property(q => q.Answer)
                .IsRequired();

            // Relationships
            builder.HasMany(q => q.VInterviewQuestions)
                .WithOne(vq => vq.VQuestions)
                .HasForeignKey(vq => vq.VQuestionID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
