namespace Optern.Infrastructure.Persistence.Configurations
{
    public class VInterviewQuestionsConfiguration : IEntityTypeConfiguration<VInterviewQuestions>
    {
        public void Configure(EntityTypeBuilder<VInterviewQuestions> builder)
        {

            #region Attributes
            // Table Name
            builder.ToTable("VInterviewQuestions");

            // Primary Key
               builder.HasKey(vq => vq.Id);
            builder.Property(vq => vq.Id)
             .ValueGeneratedOnAdd();
            
            // Properties

            builder.Property(vq => vq.VInterviewID)
                .IsRequired();

            builder.Property(vq => vq.VQuestionID)
                .IsRequired();

            #endregion

            #region Relations

            builder.HasOne(vq => vq.VInterview)
                .WithMany(v => v.VInterviewQuestions)
                .HasForeignKey(vq => vq.VInterviewID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(vq => vq.VQuestions)
                .WithMany(q => q.VInterviewQuestions)
                .HasForeignKey(vq => vq.VQuestionID)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
