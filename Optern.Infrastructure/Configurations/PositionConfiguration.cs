namespace Optern.Infrastructure.Persistence.Configurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            #region Attributes

            // Table Name

            builder.ToTable("Positions");

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


            builder.HasMany(p=> p.RoomPositions)
                .WithOne(r=>r.Position)
                .HasForeignKey(r => r.PositionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Users)
                .WithOne(u => u.Position)
                .HasForeignKey(u => u.PositionId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
