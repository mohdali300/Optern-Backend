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
    public class NotificationsConfiguration : IEntityTypeConfiguration<Notifications>
    {
        public void Configure(EntityTypeBuilder<Notifications> builder)
        {
            builder.HasMany(n => n.UserNotification)
                   .WithOne(un => un.Notifications) 
                   .HasForeignKey(un => un.NotificationId) 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Room)
             .WithMany(r => r.Notifications)
             .HasForeignKey(n => n.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
