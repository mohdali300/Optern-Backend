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
    public class BookMarkedTaskConfiguration:IEntityTypeConfiguration<BookMarkedTask>
    {
        public void Configure(EntityTypeBuilder<BookMarkedTask> builder)
        {
            #region Attributes
            builder.ToTable("BookMarkedTasks");

            //PK
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd(); 
            #endregion

            #region Relations
            builder.HasOne(b => b.UserRoom)
                    .WithMany(u => u.BookMarkedTasks)
                    .HasForeignKey(b => b.UserRoomId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Task)
                .WithMany(t => t.BookMarkedTasks)
                .HasForeignKey(b => b.TaskId)
                .OnDelete(DeleteBehavior.Cascade); 
            #endregion
        }
    }
}
