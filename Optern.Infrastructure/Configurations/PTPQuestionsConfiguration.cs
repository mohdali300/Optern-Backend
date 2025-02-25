using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class PTPQuestionsConfiguration : IEntityTypeConfiguration<PTPQuestions>
    {
        public void Configure(EntityTypeBuilder<PTPQuestions> builder)
        {
            #region Attributes
            // Table Name

            builder.ToTable("PTPQuestions");

            // Primary Key

            builder.HasKey(q => q.Id);
            builder.Property(q => q.Id)
                   .ValueGeneratedOnAdd();
            // Properties


            builder.Property(q => q.Content)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(q => q.Answer)
                   .IsRequired();


            #endregion

            #region Relations

            builder.HasMany(q => q.PTPQuestionInterviews)
                .WithOne(qi => qi.PTPQuestion)
                .HasForeignKey(qi => qi.PTPQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
