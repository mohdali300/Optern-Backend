using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Configurations
{
    internal class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {

            builder.HasOne(p => p.Creator)
                   .WithMany(u => u.CreatedPosts) 
                   .HasForeignKey(p => p.CreatorId) 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reacts)
              .WithOne(r => r.Post)
              .HasForeignKey(r => r.PostId)
              .OnDelete(DeleteBehavior.Cascade);


            builder.HasMany(p => p.Comments)
               .WithOne(c => c.Post) 
               .HasForeignKey(c => c.PostId) 
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.FavoritePosts)
              .WithOne(fp => fp.Post) 
              .HasForeignKey(fp => fp.PostId) 
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PostTags)
               .WithOne(pt => pt.Post) 
               .HasForeignKey(pt => pt.PostId) 
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
