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
    public class SprintConfiguration : IEntityTypeConfiguration<Sprint>
    {
        public void Configure(EntityTypeBuilder<Sprint> builder)
        {
            builder.HasOne(sprint => sprint.WorkSpace)
                   .WithMany(ws => ws.Sprints)
                   .HasForeignKey(sprint => sprint.WorkSpaceId)
                   .OnDelete(DeleteBehavior.Cascade);
 
            builder.HasMany(sprint => sprint.Tasks)
               .WithOne(task => task.Sprint)
               .HasForeignKey(task => task.SprintId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
