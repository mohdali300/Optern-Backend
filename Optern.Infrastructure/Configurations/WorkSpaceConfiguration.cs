namespace Optern.Infrastructure.Configurations
{
    public class WorkSpaceConfiguration : IEntityTypeConfiguration<WorkSpace>
    {
        public void Configure(EntityTypeBuilder<WorkSpace> builder)
        {

            #region Attributes

            //Table Name

            builder.ToTable("WorkSpaces", t => t.HasCheckConstraint("CK_WorkSpaces_CreatedDate_Future", "CreatedDate > NOW()"));

            // Primary key
            builder.HasKey(ws => ws.Id);
            builder.Property(ws => ws.Id)
              .ValueGeneratedOnAdd();

            // Properties
            builder.Property(ws => ws.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(ws => ws.CreatedDate)
                .IsRequired().HasDefaultValueSql("NOW()");

            // Indexes
            builder.HasIndex(ws => ws.RoomId).HasDatabaseName("IX_WorkSpace_RoomId");

            #endregion

            #region Relations
            builder.HasOne(ws => ws.Room)
                   .WithMany(r => r.WorkSpaces)
                   .HasForeignKey(ws => ws.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(ws => ws.Sprints)
              .WithOne(sprint => sprint.WorkSpace)
              .HasForeignKey(sprint => sprint.WorkSpaceId)
              .OnDelete(DeleteBehavior.Cascade);
            #endregion

        }
    }
}
