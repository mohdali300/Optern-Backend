using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        #region Attributes

        // Table Name

        builder.ToTable("Comments");


        // Primary Key 

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
       .ValueGeneratedOnAdd();

        //Properties

        builder.Property(c => c.Content)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(c => c.CommentDate)
               .IsRequired().HasDefaultValueSql("NOW()");

        builder.Property(c => c.Type)
                  .IsRequired()
                  .HasConversion<string>().HasMaxLength(20);

        builder.Property(c => c.PostId)
           .IsRequired();

        builder.Property(c => c.ParentId)
               .IsRequired(false);

        // Indexes
        builder.HasIndex(c => c.CommentDate)
       .HasDatabaseName("IX_Comments_CommentDate") 
       .IsUnique(false); 

        #endregion

        #region Relations


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
        #endregion
    }
}
