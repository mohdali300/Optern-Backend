namespace Optern.Infrastructure.Persistence.Configurations
{
    public class VQuestionsConfiguration : IEntityTypeConfiguration<VQuestions>
    {
        public void Configure(EntityTypeBuilder<VQuestions> builder)
        {

            #region Attributes
            // Table Name
            builder.ToTable("VQuestions");

            // Primary Keys
            builder.HasKey(q => q.Id);
            builder.Property(q=> q.Id)
                  .ValueGeneratedOnAdd();

            // Properties

            builder.Property(q => q.Content)
          .IsRequired()
          .HasMaxLength(500)
          .HasComment("Content of the question with a maximum of 500 characters.");

            builder.Property(q => q.Answer)
                .IsRequired();
            #endregion

            #region Relations
            builder.HasMany(q => q.VInterviewQuestions)
                .WithOne(vq => vq.VQuestions)
                .HasForeignKey(vq => vq.VQuestionID)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
