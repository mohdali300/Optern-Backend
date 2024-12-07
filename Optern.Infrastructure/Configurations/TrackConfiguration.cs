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

            // Indexes 
            builder.HasIndex(t => t.Name).HasDatabaseName("IX_Track_Name");

            #endregion

            #region Relations
            builder.HasMany(t => t.SubTracks)
                .WithOne(st => st.Track)
                .HasForeignKey(st => st.TrackId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
