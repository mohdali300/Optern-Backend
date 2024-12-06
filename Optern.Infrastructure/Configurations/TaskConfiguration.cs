using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;
using Task = Optern.Domain.Entities.Task;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {

        builder.ToTable("Tasks");

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Id)
            .IsRequired();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.StartDate)
            .IsRequired();

        builder.Property(t => t.DueDate)
            .IsRequired();

        builder.Property(t => t.EndDate);

        builder.Property(t => t.Status)
            .IsRequired();

        // Relations
        builder.HasMany(t => t.AssignedTasks)
               .WithOne(ut => ut.Task) 
               .HasForeignKey(ut => ut.TaskId) 
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(task => task.Sprint)
              .WithMany(sprint => sprint.Tasks)
              .HasForeignKey(task => task.SprintId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
