namespace Optern.Infrastructure.Configurations
{
    public class RoomTrackConfiguration : IEntityTypeConfiguration<RoomTrack>
    {
        public void Configure(EntityTypeBuilder<RoomTrack> builder)
        {
            #region Attributes
            // Table Name
            builder.ToTable("RoomTracks");

            // Primary Key

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                   .ValueGeneratedOnAdd(); 
            #endregion

            #region Relations
            builder.HasOne(rt => rt.Room)
                 .WithMany(r => r.RoomTracks)
                 .HasForeignKey(rt => rt.RoomId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rt => rt.Track)
              .WithMany(st => st.RoomTracks)
              .HasForeignKey(rt => rt.TrackId)
              .OnDelete(DeleteBehavior.Cascade); 
            #endregion
        }
    }
}
