using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            #region Attributes
            // Table Name


            builder.ToTable("Tracks");

            // Primary Key
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
              .ValueGeneratedOnAdd();

            // Properties
            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            #endregion

            #region Relations
            builder.HasMany(t => t.RoomTracks)
                .WithOne(rt => rt.Track)
                .HasForeignKey(rt => rt.TrackId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
