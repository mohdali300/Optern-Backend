namespace Optern.Infrastructure.Persistence.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {

            #region Attributes
            // Table Name
            builder.ToTable("Rooms", r => r.HasCheckConstraint("CK_Room_Capacity", "\"Capacity\" >= 1"));

            // Primary Key

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                   .ValueGeneratedOnAdd();

            // Properties

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r=>r.RoomType).IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(c=>c.CreatorId)
             .IsRequired();


            // Indexes

            builder.HasIndex(r => r.Name)
                .HasDatabaseName("IX_Rooms_Name");

            builder.HasIndex(r => r.CreatorId)
                .HasDatabaseName("IX_Rooms_CreatorId");

            #endregion


            #region Relations
            builder.HasOne(r => r.Creator)
                .WithMany(u => u.Rooms)
                .HasForeignKey(r => r.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.UserRooms)
                .WithOne(ur => ur.Room)
                .HasForeignKey(ur => ur.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Notifications)
                .WithOne(r=>r.Room)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.WorkSpaces)
                .WithOne(r => r.Room)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.RoomTracks)
                .WithOne(rt => rt.Room)
                .HasForeignKey(rt => rt.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.RoomPositions)
                .WithOne(rp => rp.Room)
                .HasForeignKey(rp => rp.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
