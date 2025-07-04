﻿namespace Optern.Infrastructure.Configurations
{
    public class CVConfiguration : IEntityTypeConfiguration<CV>
    {
        public void Configure(EntityTypeBuilder<CV> builder)
        {

            #region Attributes
            // Table Name

            builder.ToTable("CV");


            // Primary Key 

            builder.HasKey(cv => cv.Id);
            builder.Property(cv => cv.Id)
          .ValueGeneratedOnAdd();

            //Properties


            builder.Property(cv => cv.Title)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(cv => cv.FilePath)
                   .HasMaxLength(255);

            builder.Property(cv => cv.UserId)
                .IsRequired();

            #endregion

            #region Relations

            builder.HasOne(cv => cv.User) 
                   .WithMany(u => u.CVs) 
                   .HasForeignKey(cv => cv.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
