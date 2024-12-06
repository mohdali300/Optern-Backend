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
    internal class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {

            // one user create many Chats 
            builder.HasOne(c => c.Creator)
               .WithMany(u => u.CreatedChats)
               .HasForeignKey(c => c.CreatorId)
               .OnDelete(DeleteBehavior.NoAction);

            // m users can joined to m chats
            builder.HasMany(c => c.ChatParticipants)
                 .WithOne(cp => cp.Chat)
                 .HasForeignKey(cp => cp.ChatId) 
                 .OnDelete(DeleteBehavior.Cascade);
            // 1 to 1 relation , each room has exactly one chat
            builder.HasOne(c => c.Room)
                 .WithOne(r => r.Chat)
                 .HasForeignKey<Chat>(c => c.RoomId) 
                 .OnDelete(DeleteBehavior.Cascade);
            // 1 to m , each chat has many Messages
            builder.HasMany(c => c.Messages)
                 .WithOne(m => m.Chat)
                 .HasForeignKey(m => m.ChatId)
                 .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
