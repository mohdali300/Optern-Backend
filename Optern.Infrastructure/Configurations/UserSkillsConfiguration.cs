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
    public class UserSkillsConfiguration : IEntityTypeConfiguration<UserSkills>
    {
        public void Configure(EntityTypeBuilder<UserSkills> builder)
        {

            #region Attributes
            // Table Name

            builder.ToTable("UserSkills");

            // Primary Key
            builder.HasKey(us => us.Id);
            builder.Property(us => us.Id)
             .ValueGeneratedOnAdd();

            //properties
            builder.Property(us => us.UserId)
                   .IsRequired();

            builder.Property(us => us.SkillId)
                   .IsRequired();

            // Indexes

            builder.HasIndex(us => new { us.UserId, us.SkillId })
            .IsUnique()
            .HasDatabaseName("IX_UserSkills_User_Skill");
            #endregion

            #region Relations
            builder.HasOne(us => us.User)
                   .WithMany(u => u.UserSkills) 
                   .HasForeignKey(us => us.UserId) 
                   .OnDelete(DeleteBehavior.Cascade); 


            builder.HasOne(us => us.Skill)
                   .WithMany(s => s.UserSkills) 
                   .HasForeignKey(us => us.SkillId) 
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
