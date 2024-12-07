using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

public class UserTasksConfiguration : IEntityTypeConfiguration<UserTasks>
{
    public void Configure(EntityTypeBuilder<UserTasks> builder)
    {
        #region Attributes

        // Table Name

        builder.ToTable("UserTasks");


        // Primary Key
        builder.HasKey(ut => ut.Id);
        builder.Property(ut => ut.Id)
               .ValueGeneratedOnAdd();

        // Properties

        builder.Property(ut => ut.UserId)
               .IsRequired();

        builder.Property(ut => ut.TaskId)
               .IsRequired();
        // Indexes

        builder.HasIndex(ut => new { ut.UserId, ut.TaskId })
              .IsUnique()
              .HasDatabaseName("UX_UserTasks_User_Task");
        #endregion

        #region Relations
        builder.HasOne(ut => ut.User)
               .WithMany(u => u.AssignedTasks) 
               .HasForeignKey(ut => ut.UserId) 
               .OnDelete(DeleteBehavior.Cascade); 


        builder.HasOne(ut => ut.Task)
               .WithMany(t => t.AssignedTasks) 
               .HasForeignKey(ut => ut.TaskId) 
               .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}
