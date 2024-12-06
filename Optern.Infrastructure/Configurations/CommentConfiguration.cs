using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        // Attributes

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(c => c.CommentDate)
               .IsRequired();

        builder.Property(c => c.PostId)
           .IsRequired();

        builder.Property(c => c.ParentId)
               .IsRequired(false);

        // Relations


        builder.HasOne(c => c.User)
               .WithMany(u => u.Comments)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Post)
               .WithMany(p => p.Comments)
               .HasForeignKey(c => c.PostId)
               .OnDelete(DeleteBehavior.Cascade);

        // self relation
        builder.HasOne(c => c.comment)
               .WithMany()
               .HasForeignKey(c => c.ParentId)
               .OnDelete(DeleteBehavior.Cascade); // remove all replied comments if parent is deleted

        // reacts on comments
        builder.HasMany(c => c.CommentReacts)
              .WithOne(cr => cr.Comment) 
              .HasForeignKey(cr => cr.CommentId) 
              .OnDelete(DeleteBehavior.Cascade);
    }
}
