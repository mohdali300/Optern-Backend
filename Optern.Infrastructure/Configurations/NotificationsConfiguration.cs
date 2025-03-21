namespace Optern.Infrastructure.Configurations
{
    public class NotificationsConfiguration : IEntityTypeConfiguration<Notifications>
    {
        public void Configure(EntityTypeBuilder<Notifications> builder)
        {

            #region Attributes

            //Table Name
            builder.ToTable("Notifications");

            // Primary Key

            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
              .ValueGeneratedNever();

            // Properties

            builder.Property(n => n.Title)
                   .HasDefaultValue(string.Empty)
                   .HasMaxLength(100);

            builder.Property(n => n.Message)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(n => n.CreatedTime)
                   .HasDefaultValueSql("NOW()");

            builder.Property(r => r.RoomId)
               .IsRequired(false);


            #endregion

            #region Relations
            builder.HasMany(n => n.UserNotification)
                   .WithOne(un => un.Notifications) 
                   .HasForeignKey(un => un.NotificationId) 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Room)
             .WithMany(r => r.Notifications)
             .HasForeignKey(n => n.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
