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
            #region Attributes

            // Table Name
            builder.ToTable("Sprints", s => s.HasCheckConstraint("CK_Sprint_StartEndDate", "[StartDate] < [EndDate]"));

            // Primary Key
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                 .ValueGeneratedOnAdd();

            // Properties

            builder.Property(s=> s.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(s => s.StartDate)
                   .IsRequired();

            builder.Property(s => s.EndDate)
                   .IsRequired();

            builder.Property(s => s.WorkSpaceId)
                  .IsRequired();
            #endregion

            #region Relations
            builder.HasOne(sprint => sprint.WorkSpace)
                   .WithMany(ws => ws.Sprints)
                   .HasForeignKey(sprint => sprint.WorkSpaceId)
                   .OnDelete(DeleteBehavior.Cascade);
 
            builder.HasMany(sprint => sprint.Tasks)
               .WithOne(task => task.Sprint)
               .HasForeignKey(task => task.SprintId)
               .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }

}
