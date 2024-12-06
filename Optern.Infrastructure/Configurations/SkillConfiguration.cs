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
            builder.HasMany(s => s.UserSkills)
                   .WithOne(us => us.Skill) 
                   .HasForeignKey(us => us.SkillId) 
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
