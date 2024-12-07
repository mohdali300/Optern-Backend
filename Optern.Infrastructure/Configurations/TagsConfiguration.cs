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
    public class TagsConfiguration : IEntityTypeConfiguration<Tags>
    {
        public void Configure(EntityTypeBuilder<Tags> builder)
        {

            #region Attributes

            // Table Name

            builder.ToTable("Tags");

            // Primary Key

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                   .ValueGeneratedOnAdd();
            // Properties

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(50);
            // Indexes

            builder.HasIndex(t => t.Name).HasDatabaseName("IX_Tags_Name");

            #endregion

            #region Relations
            builder.HasMany(t => t.PostTags)
                   .WithOne(pt => pt.Tag) 
                   .HasForeignKey(pt => pt.TagId) 
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }

}
