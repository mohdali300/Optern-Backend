using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Configurations
{
    public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
    {
        public void Configure(EntityTypeBuilder<Experience> builder)
        {
            #region Attributes
            // Table Name
            builder.ToTable("Experience");


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

            // Indexes


            builder.HasIndex(e => new { e.UserId, e.Company, e.JobTitle, e.StartDate })
           .IsUnique();

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
