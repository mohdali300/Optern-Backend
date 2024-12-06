using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.ToTable("UserNotifications");

            builder.HasKey(un => un.Id);

            builder.Property(un => un.Id)
                .IsRequired();

 

            builder.Property(un => un.NotificationId)
                .IsRequired();

            // Relationships

            builder.HasOne(un => un.User)
                .WithMany(u => u.UserNotification) 
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(un => un.Notifications)
                .WithMany(n => n.UserNotification) 
                .HasForeignKey(un => un.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
