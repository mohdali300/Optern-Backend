using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class PTPQuestionsConfiguration : IEntityTypeConfiguration<PTPQuestions>
    {
        public void Configure(EntityTypeBuilder<PTPQuestions> builder)
        {
            builder.ToTable("PTPQuestions");

            // Primary Key
            builder.HasKey(q => q.Id);
 

            // Relationships
            builder.HasMany(q => q.PTPQuestionInterviews)
                .WithOne(qi => qi.PTPQuestion)
                .HasForeignKey(qi => qi.PTPQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
