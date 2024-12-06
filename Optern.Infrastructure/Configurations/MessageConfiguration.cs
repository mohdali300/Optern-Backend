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
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
           
            builder.HasOne(m => m.Sender)
                   .WithMany(u => u.Messages)
                   .HasForeignKey(m => m.SenderId) 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Chat)
              .WithMany(c => c.Messages)
              .HasForeignKey(m => m.ChatId)
              .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
