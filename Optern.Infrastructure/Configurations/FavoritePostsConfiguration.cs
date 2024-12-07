using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

public class FavoritePostsConfiguration : IEntityTypeConfiguration<FavoritePosts>
{
    public void Configure(EntityTypeBuilder<FavoritePosts> builder)
    {

        #region Attributes

        //Table Name

        builder.ToTable("FavoritePosts");

        // Primary Key

        builder.HasKey(fp => fp.Id);
        builder.Property(fp => fp.Id)
               .ValueGeneratedOnAdd();
        // Indexes

        builder.HasIndex(fp => new { fp.UserId, fp.PostId })
        .IsUnique()
        .HasDatabaseName("IX_FavoritePosts_UserId_PostId");

        #endregion

        #region Relations
        builder.HasOne(fp => fp.User)
               .WithMany(u => u.FavoritePosts)
               .HasForeignKey(fp => fp.UserId) 
               .OnDelete(DeleteBehavior.Cascade); 


        builder.HasOne(fp => fp.Post)
               .WithMany(p => p.FavoritePosts) 
               .HasForeignKey(fp => fp.PostId)
               .OnDelete(DeleteBehavior.Cascade);
        #endregion
    }
}
