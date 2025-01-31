using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;
using Task = Optern.Domain.Entities.Task;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        #region Attributes
        // Table Name

        builder.ToTable("Tasks");

        // Primary Key
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
                .ValueGeneratedOnAdd();

        // Properties
        builder.Property(t => t.Title)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(t => t.Description)
               .HasMaxLength(1000);

        builder.Property(t => t.StartDate)
               .IsRequired();

        builder.Property(t => t.DueDate)
               .IsRequired();

        builder.Property(t => t.EndDate);

        builder.Property(t => t.Status)
               .IsRequired().HasConversion<string>();

        builder.Property(s => s.SprintId)
               .IsRequired();

        // Indexes
        builder.HasIndex(t => t.SprintId)
            .HasDatabaseName("IX_Task_SprintId");

        #endregion

        #region Relations

        builder.HasMany(t => t.AssignedTasks)
               .WithOne(ut => ut.Task) 
               .HasForeignKey(ut => ut.TaskId) 
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.BookMarkedTasks)
               .WithOne(b => b.Task)
               .HasForeignKey(b => b.TaskId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(task => task.Sprint)
              .WithMany(sprint => sprint.Tasks)
              .HasForeignKey(task => task.SprintId)
              .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}
