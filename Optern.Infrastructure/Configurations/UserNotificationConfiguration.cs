using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            #region Attributes

            // Table Name
            builder.ToTable("UserNotifications");

            // Primary Key
            builder.HasKey(un => un.Id);
            builder.Property(un => un.Id)
              .ValueGeneratedOnAdd();
            #endregion

            #region Relations
            builder.HasOne(un => un.User)
                .WithMany(u => u.UserNotification) 
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(un => un.Notifications)
                .WithMany(n => n.UserNotification) 
                .HasForeignKey(un => un.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
