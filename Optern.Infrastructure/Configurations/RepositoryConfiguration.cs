namespace Optern.Infrastructure.Configurations
{
    public class RepositoryConfiguration : IEntityTypeConfiguration<Repository>
    {
        public void Configure(EntityTypeBuilder<Repository> builder)
        {
            #region Attributes
            // Table Name
            builder.ToTable("Repository");

            //Primary Key
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
             .ValueGeneratedOnAdd();

            // Properties
            builder.Property(r=> r.RoomId)
               .IsRequired();
            #endregion

            #region Relations

            builder.HasOne(r => r.Room)
                   .WithOne(ro => ro.Repository)
                   .HasForeignKey<Repository>(r => r.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            #endregion



        }
    }

}
