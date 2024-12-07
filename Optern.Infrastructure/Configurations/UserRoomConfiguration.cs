using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;

namespace Optern.Infrastructure.Configurations
{
    public class UserRoomConfiguration : IEntityTypeConfiguration<UserRoom>
    {
        public void Configure(EntityTypeBuilder<UserRoom> builder)
        {

            #region Attributes
            // Table Name

            builder.ToTable("UserRoom");

            // Primary Key

            builder.HasKey(ur => ur.Id);
            builder.Property(ur => ur.Id)
            .ValueGeneratedOnAdd();

            // indexes

            builder.HasIndex(ur => new { ur.RoomId, ur.UserId })
             .IsUnique()
             .HasDatabaseName("UX_UserRoom_Room_User");
            #endregion

            #region Relations

            builder.HasOne(ur => ur.Room)
                   .WithMany(r => r.UserRooms)
                   .HasForeignKey(ur => ur.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.UserRooms)
                   .HasForeignKey(ur => ur.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
