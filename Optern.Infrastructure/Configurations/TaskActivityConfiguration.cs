namespace Optern.Infrastructure.Configurations
{
    public class TaskActivityConfiguration : IEntityTypeConfiguration<TaskActivity>
    {
        public void Configure(EntityTypeBuilder<TaskActivity> builder)
        {
            #region Attributes
            builder.ToTable("TaskActivity");

            
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();
            builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(500); 

            builder.Property(e => e.CreatedAt)
            .IsRequired();

            builder.Property(e => e.CouldDelete)
                .IsRequired();
            #endregion

            #region Relations
            builder.HasOne(e => e.Task)
           .WithMany(t => t.Activities)
           .HasForeignKey(e => e.TaskId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ta => ta.Creator)
               .WithMany(u => u.TaskActivities)
               .HasForeignKey(ta => ta.CreatorId)
               .OnDelete(DeleteBehavior.SetNull);
            #endregion
        }
    }
}
