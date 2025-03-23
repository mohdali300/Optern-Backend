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

            builder.Property(u => u.UserId)
                .IsRequired();

            builder.Property(n=>n.NotificationId)
                .IsRequired();


            builder.Property(n => n.CreatedTime)
                   .HasDefaultValueSql("NOW()");
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
