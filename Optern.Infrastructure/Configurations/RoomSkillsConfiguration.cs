namespace Optern.Infrastructure.Configurations
{
    public class RoomSkillsConfiguration : IEntityTypeConfiguration<RoomSkills>
    {

        public void Configure(EntityTypeBuilder<RoomSkills> builder)
        {
            #region Attributes

            // Table Name
            builder.ToTable("RoomSkills");

            // Primary Key
            builder.HasKey(rs => rs.Id);
            builder.Property(rs => rs.Id)
                    .ValueGeneratedOnAdd();
            // Properties
            builder.Property(rs => rs.RoomId)
                .IsRequired();

            builder.Property(rs => rs.SkillId)
                .IsRequired();

            // Indexes

            builder.HasIndex(us => new { us.RoomId, us.SkillId })
            .IsUnique()
            .HasDatabaseName("IX_RoomSkills_Room_Skill");
            #endregion
            #region Relations

            builder.HasOne(rs => rs.Room)
                .WithMany(r => r.RoomSkills)
                .HasForeignKey(rs => rs.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rs => rs.Skill)
                .WithMany(s => s.RoomSkills)
                .HasForeignKey(rs => rs.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
