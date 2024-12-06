using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

public class FavoritePostsConfiguration : IEntityTypeConfiguration<FavoritePosts>
{
    public void Configure(EntityTypeBuilder<FavoritePosts> builder)
    {
     

        builder.HasKey(fp => fp.Id);

        builder.HasOne(fp => fp.User)
               .WithMany(u => u.FavoritePosts)
               .HasForeignKey(fp => fp.UserId) 
               .OnDelete(DeleteBehavior.Cascade); 


        builder.HasOne(fp => fp.Post)
               .WithMany(p => p.FavoritePosts) 
               .HasForeignKey(fp => fp.PostId)
               .OnDelete(DeleteBehavior.Cascade); 
    }
}
