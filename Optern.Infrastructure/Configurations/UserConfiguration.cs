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
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Attributes

            builder.Property(u => u.FirstName)
             .IsRequired()
             .HasMaxLength(50);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.Role)
                   .IsRequired();

            builder.Property(u => u.JobTitle)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Property(u => u.ProfilePicture)
                .IsRequired(false);

            builder.Property(u => u.Email)
                .IsRequired();
            
            builder.Property(u => u.Gender)
                .IsRequired(false);  
            
            builder.Property(u => u.Country)
                .IsRequired(false);
            
            builder.Property(u => u.CreatedAt)
                .IsRequired(false); 

            builder.Property(u => u.LastLogIn)
                .IsRequired(false);



            // Relations

            builder.HasMany(r => r.Rooms)
                .WithOne(r => r.Creator)
                .HasForeignKey(u => u.CreatorId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
