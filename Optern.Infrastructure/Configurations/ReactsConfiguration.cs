using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class ReactsConfiguration : IEntityTypeConfiguration<Reacts>
    {
        public void Configure(EntityTypeBuilder<Reacts> builder)
        {
            builder.ToTable("Reacts");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReactDate)
                .IsRequired();

            builder.Property(r => r.ReactType)
                .IsRequired();

            builder.Property(r => r.UserId)
                .IsRequired()
                .HasMaxLength(450); 

            builder.Property(r => r.PostId)
                .IsRequired();

            // Relationships

            builder.HasOne(r => r.User)
                .WithMany(u => u.Reacts) 
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Post)
                .WithMany(p => p.Reacts) 
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
