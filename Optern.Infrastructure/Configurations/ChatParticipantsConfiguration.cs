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
    public class ChatParticipantsConfiguration : IEntityTypeConfiguration<ChatParticipants>
    {
        public void Configure(EntityTypeBuilder<ChatParticipants> builder)
        {
            builder.HasKey(cp => cp.Id);

            builder.HasOne(cp => cp.Chat)
                   .WithMany(c => c.ChatParticipants)
                   .HasForeignKey(cp => cp.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.User)
                   .WithMany(u => u.JoinedChats)
                   .HasForeignKey(cp => cp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(cp => cp.JoinedAt)
                   .IsRequired();
        }
    }
}