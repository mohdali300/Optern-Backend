﻿namespace Optern.Infrastructure.Configurations
{
    public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
    {
        public void Configure(EntityTypeBuilder<Experience> builder)
        {
            #region Attributes
            // Table Name
            builder.ToTable("Experiences");


            // Primary Key 
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
             .ValueGeneratedOnAdd();

            //Properties


            builder.Property(e => e.JobTitle)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Company)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.JobDescription)
                .HasMaxLength(500);

            builder.Property(e => e.Location)
                .HasMaxLength(100);

            builder.Property(e => e.UserId)
               .IsRequired();

            // Indexes

            builder.HasIndex(e =>e.UserId)
                .HasDatabaseName("IX_Experience_UserId")
                .IsUnique(false);

            #endregion

            #region Relations
            builder.HasOne(e => e.User)
                   .WithMany(u => u.Experiences) 
                   .HasForeignKey(e => e.UserId) 
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }

}
