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
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {

            #region Attributes 

            // Table Name

            builder.ToTable("Post"); 
            

            // Primary Key
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            // Properties

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Content)
                .IsRequired().HasConversion<string>()
                .HasMaxLength(5000);

            builder.Property(p => p.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(p => p.CreatorId)
                .IsRequired();

            // Indexes

            builder.HasIndex(p => new { p.CreatorId, p.CreatedDate })
                   .HasDatabaseName("IX_Posts_Creator_CreatedDate");

            builder.HasIndex(p => p.Title)
                   .HasDatabaseName("IX_Posts_Title");

            builder.HasIndex(p => p.CreatedDate)
                   .HasDatabaseName("IX_Posts_CreatedDate");

            #endregion

            #region Relations

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
            #endregion
        }
    }
}
