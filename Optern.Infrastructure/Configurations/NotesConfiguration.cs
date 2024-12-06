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
    public class NotesConfiguration : IEntityTypeConfiguration<Notes>
    {
        public void Configure(EntityTypeBuilder<Notes> builder)
        {

            // attributes

            builder.Property(n => n.Content)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(n => n.Room)
                   .WithMany(r => r.Notes)
                   .HasForeignKey(n => n.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

           

        }
    }

}
