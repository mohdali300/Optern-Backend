using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            // Table Name
            builder.ToTable("Rooms");

            // Primary Key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.Id)
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r => r.Capacity)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(r => r.Creator)
                .WithMany(u => u.Rooms)
                .HasForeignKey(r => r.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.SubTracks)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.UserRooms)
                .WithOne(ur => ur.Room)
                .HasForeignKey(ur => ur.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Notes)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Notifications)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.WorkSpaces)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
