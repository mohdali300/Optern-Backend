using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Configurations
{
    public class EducationConfiguration : IEntityTypeConfiguration<Education>
    {
        public void Configure(EntityTypeBuilder<Education> builder)
        {

            #region Attributes

            // Table Name

            builder.ToTable("Educations");

            //Properties

            builder.HasKey(e => e.Id);
            builder.Property(e =>e.Id)
              .ValueGeneratedOnAdd();

            //Properties


            builder.Property(e => e.School)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.University)
                .HasMaxLength(150);

            builder.Property(e => e.Major)
                .HasMaxLength(100);

            builder.Property(e => e.StartYear)
                .IsRequired();

            builder.Property(e => e.EndYear)
                .IsRequired();

            builder.Property(e => e.Degree)
                .IsRequired()
                .HasConversion<string>().HasMaxLength(150);

            // Indexes

            builder.HasIndex(e => e.UserId)
                   .HasDatabaseName("IX_Education_UserId");

            builder.HasIndex(e => new { e.UserId, e.Degree })
                   .HasDatabaseName("IX_Education_UserId_Degree");

            builder.HasIndex(e => e.University)
                   .HasDatabaseName("IX_Education_University");


            #endregion

            #region Relations
            builder.HasOne(e => e.User)
                   .WithMany(u => u.Educations) 
                   .HasForeignKey(e => e.UserId) 
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion

        }
    }
}
