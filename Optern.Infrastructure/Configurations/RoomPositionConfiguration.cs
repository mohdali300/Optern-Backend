using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Configurations
{
    public class RoomPositionConfiguration : IEntityTypeConfiguration<RoomPosition>
    {
        public void Configure(EntityTypeBuilder<RoomPosition> builder)
        {
            #region Attributes

            // Table Name
            builder.ToTable("RoomPositions");

            // Primary Key

            builder.HasKey(rt => rt.Id);
            builder.Property(r => r.Id)
                 .ValueGeneratedOnAdd();

            builder.Property(s => s.PositionId)
                .IsRequired();

            builder.Property(s=>s.RoomId)
                .IsRequired();

            #endregion
            #region Relations


            builder.HasOne(rt => rt.Room)
             .WithMany(r => r.RoomPositions)
             .HasForeignKey(rt => rt.RoomId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rt => rt.Position)
              .WithMany(st=>st.RoomPositions)
              .HasForeignKey(rt => rt.PositionId)
              .OnDelete(DeleteBehavior.Cascade);

            #endregion
        }
    }
}
