using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Optern.Domain.Entities;

namespace Optern.Infrastructure.Persistence.Configurations
{
    public class ReactsConfiguration : IEntityTypeConfiguration<Reacts>
    {
        public void Configure(EntityTypeBuilder<Reacts> builder)
        {
            #region Attributes

            // Table Name


            builder.ToTable("Reacts");

            // Primary Key

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                  .ValueGeneratedOnAdd();
            // Properties

            builder.Property(r => r.ReactDate)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(r => r.ReactType)
                .IsRequired().HasConversion<string>();
          

            builder.Property(r => r.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(r => r.PostId)
                .IsRequired();

            // Indexes

            builder.HasIndex(r => r.UserId)
                .HasDatabaseName("IX_Reacts_UserId");

            builder.HasIndex(r => r.PostId)
                .HasDatabaseName("IX_Reacts_PostId");

            builder.HasIndex(r => r.ReactType)
                .HasDatabaseName("IX_Reacts_ReactType");
            #endregion


            #region Relations
            builder.HasOne(r => r.User)
                .WithMany(u => u.Reacts) 
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Post)
                .WithMany(p => p.Reacts) 
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
