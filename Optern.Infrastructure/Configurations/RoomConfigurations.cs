using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.Configurations
{
    public class RoomConfigurations : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            // Attributes

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.Description)
                   .HasMaxLength(500);

            builder.Property(r => r.Capacity)
                   .IsRequired();


            // Relations
            builder.HasOne(r => r.Creator)
                .WithMany(u => u.Rooms)
                .HasForeignKey(r => r.CreatorId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);
           
        }
    }
}
