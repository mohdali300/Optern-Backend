namespace Optern.Infrastructure.Persistence.Configurations
{
    public class VInterviewConfiguration : IEntityTypeConfiguration<VInterview>
    {
        public void Configure(EntityTypeBuilder<VInterview> builder)
        {
            #region Attributes

            //Table Name
            builder.ToTable("VInterview");
            // Primary Key

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id)
              .ValueGeneratedOnAdd();

            // Properties

            builder.Property(v => v.Category)
                .IsRequired().HasConversion<string>();

            builder.Property(v => v.QusestionType)
                .IsRequired().HasConversion<string>();

            

            //Indexes 
            builder.HasIndex(v => v.UserId)
                .HasDatabaseName("IX_VInterview_UserId");
            #endregion

            #region Relations
            builder.HasOne(v => v.User)
                .WithMany(u => u.JoinedVirtualInterviews)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.VirtualFeedBack)
           .WithOne()
           .HasForeignKey<VFeedBack>(vf => vf.VInterviewID) 
           .OnDelete(DeleteBehavior.Cascade);


            #endregion
        }
    }
}
