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
    public class SkillConfiguration : IEntityTypeConfiguration<Skills>
    {
        public void Configure(EntityTypeBuilder<Skills> builder)
        {
            #region Attributes

            // Table Name

            builder.ToTable("Skills");

            // Primary Key

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                  .ValueGeneratedOnAdd();

            // Properties

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes

            builder.HasIndex(s => s.Name)
                .IsUnique()
                .HasDatabaseName("IX_Skills_Name");
            #endregion

            #region Relations

            builder.HasMany(s => s.UserSkills)
                   .WithOne(us => us.Skill) 
                   .HasForeignKey(us => us.SkillId) 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(rs=>rs.RoomSkills)
                .WithOne(s=>s.Skill)
                .HasForeignKey(s=>s.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
