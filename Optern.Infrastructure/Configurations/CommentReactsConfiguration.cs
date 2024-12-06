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
    public class CommentReactsConfiguration : IEntityTypeConfiguration<CommentReacts>
    {
        public void Configure(EntityTypeBuilder<CommentReacts> builder)
        {
            builder.HasKey(x => x.Id); 
            
            builder.HasOne(cr => cr.User)
                   .WithMany(u => u.CommentReacts) 
                   .HasForeignKey(cr => cr.UserId) 
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(cr => cr.Comment)
                   .WithMany(c => c.CommentReacts) 
                   .HasForeignKey(cr => cr.CommentId) 
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }

}
