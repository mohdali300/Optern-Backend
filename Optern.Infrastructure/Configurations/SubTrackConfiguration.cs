using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class SubTrackConfiguration : IEntityTypeConfiguration<SubTrack>
    {
        public void Configure(EntityTypeBuilder<SubTrack> builder)
        {
            builder.ToTable("SubTracks");

            builder.HasKey(st => st.Id);

            builder.Property(st => st.Id)
                .IsRequired();

            builder.Property(st => st.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(st => st.Track)
                .WithMany(t => t.SubTracks)
                .HasForeignKey(st => st.TrackId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
