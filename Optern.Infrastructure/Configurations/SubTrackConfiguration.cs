using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class SubTrackConfiguration : IEntityTypeConfiguration<SubTrack>
    {
        public void Configure(EntityTypeBuilder<SubTrack> builder)
        {
            #region Attributes

            // Table Name

            builder.ToTable("SubTracks");

            // Primary Key

            builder.HasKey(st => st.Id);
            builder.Property(s => s.Id)
              .ValueGeneratedOnAdd();

            // Properties

            builder.Property(st => st.Name)
                   .IsRequired()
                   .HasMaxLength(100);
            #endregion

            #region Relations

            builder.HasOne(st => st.Track)
                .WithMany(t => t.SubTracks)
                .HasForeignKey(st => st.TrackId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
