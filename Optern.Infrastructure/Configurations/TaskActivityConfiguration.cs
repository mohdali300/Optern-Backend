using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;
using static Dapper.SqlMapper;

namespace Optern.Infrastructure.Configurations
{
    public class TaskActivityConfiguration : IEntityTypeConfiguration<TaskActivity>
    {
        public void Configure(EntityTypeBuilder<TaskActivity> builder)
        {
            #region Attributes
            builder.ToTable("TaskActivity");

            
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();
            builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(500); 

            builder.Property(e => e.CreatedAt)
            .IsRequired();

            builder.Property(e => e.CouldDelete)
                .IsRequired();
            #endregion

            #region Relations
            builder.HasOne(e => e.Task)
           .WithMany(t => t.Activities)
           .HasForeignKey(e => e.TaskId)
           .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
