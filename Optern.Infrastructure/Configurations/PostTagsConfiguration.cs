namespace Optern.Infrastructure.Configurations
{
    public class PostTagsConfiguration : IEntityTypeConfiguration<PostTags>
    {
        public void Configure(EntityTypeBuilder<PostTags> builder)
        {

            #region Attributes

            // Table Name

            builder.ToTable("PostTags");

            // Primary Key

            builder.HasKey(pt => pt.Id);
            builder.Property(pt => pt.Id)
                   .ValueGeneratedOnAdd();

            // Properties

            builder.Property(pt => pt.PostId)
                   .IsRequired();

            builder.Property(pt => pt.TagId)
                   .IsRequired();
            // Indexes

            builder.HasIndex(pt => new { pt.PostId, pt.TagId })
                   .IsUnique()
                   .HasDatabaseName("IX_PostTags_PostId_TagId");

            #endregion

            #region Relations

            builder.HasOne(pt => pt.Post)
                   .WithMany(p => p.PostTags)
                   .HasForeignKey(pt => pt.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pt => pt.Tag)
                   .WithMany(t => t.PostTags)
                   .HasForeignKey(pt => pt.TagId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }

}
