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
    public class RoomTrackConfiguration : IEntityTypeConfiguration<RoomTrack>
    {
        public void Configure(EntityTypeBuilder<RoomTrack> builder)
        {
            #region Attributes

            // Table Name
            builder.ToTable("RoomTracks");

            // Primary Key

            builder.HasKey(rt => rt.Id);
            builder.Property(r => r.Id)
                 .ValueGeneratedOnAdd();


            #endregion
            #region Relations


            builder.HasOne(rt => rt.Room)
             .WithMany(r => r.RoomTracks)
             .HasForeignKey(rt => rt.RoomId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rt => rt.SubTrack)
              .WithMany(st=>st.RoomTracks)
              .HasForeignKey(rt => rt.SubTrackId)
              .OnDelete(DeleteBehavior.Cascade);

            #endregion
        }
    }
}
