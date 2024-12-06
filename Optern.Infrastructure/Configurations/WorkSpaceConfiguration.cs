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
    public class WorkSpaceConfiguration : IEntityTypeConfiguration<WorkSpace>
    {
        public void Configure(EntityTypeBuilder<WorkSpace> builder)
        {
            builder.HasOne(ws => ws.Room)
                   .WithMany(r => r.WorkSpaces)
                   .HasForeignKey(ws => ws.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(ws => ws.Sprints)
              .WithOne(sprint => sprint.WorkSpace)
              .HasForeignKey(sprint => sprint.WorkSpaceId)
              .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
