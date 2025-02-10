namespace Optern.Infrastructure.Configurations
{
    public class BookMarkedTaskConfiguration:IEntityTypeConfiguration<BookMarkedTask>
    {
        public void Configure(EntityTypeBuilder<BookMarkedTask> builder)
        {
            #region Attributes
            builder.ToTable("BookMarkedTasks");

            //PK
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd(); 
            #endregion

            #region Relations
            builder.HasOne(b => b.User)
                    .WithMany(u => u.BookMarkedTasks)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Task)
                .WithMany(t => t.BookMarkedTasks)
                .HasForeignKey(b => b.TaskId)
                .OnDelete(DeleteBehavior.Cascade); 
            #endregion
        }
    }
}
