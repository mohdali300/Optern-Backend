using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

public class UserTasksConfiguration : IEntityTypeConfiguration<UserTasks>
{
    public void Configure(EntityTypeBuilder<UserTasks> builder)
    {
        builder.HasOne(ut => ut.User)
               .WithMany(u => u.AssignedTasks) 
               .HasForeignKey(ut => ut.UserId) 
               .OnDelete(DeleteBehavior.Cascade); 


        builder.HasOne(ut => ut.Task)
               .WithMany(t => t.AssignedTasks) 
               .HasForeignKey(ut => ut.TaskId) 
               .OnDelete(DeleteBehavior.Cascade); 
    }
}
