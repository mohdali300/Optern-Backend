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
            builder.HasMany(t => t.PostTags)
                   .WithOne(pt => pt.Tag) 
                   .HasForeignKey(pt => pt.TagId) 
                   .OnDelete(DeleteBehavior.Cascade);         
        }
    }

}
