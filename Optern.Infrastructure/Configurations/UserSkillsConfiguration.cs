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
            builder.HasOne(us => us.User)
                   .WithMany(u => u.UserSkills) 
                   .HasForeignKey(us => us.UserId) 
                   .OnDelete(DeleteBehavior.Cascade); 


            builder.HasOne(us => us.Skill)
                   .WithMany(s => s.UserSkills) 
                   .HasForeignKey(us => us.SkillId) 
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
