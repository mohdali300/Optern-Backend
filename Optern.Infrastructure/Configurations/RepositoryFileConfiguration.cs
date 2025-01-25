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
    public class RepositoryFileConfiguration:IEntityTypeConfiguration<RepositoryFile>
    {
        public void Configure(EntityTypeBuilder<RepositoryFile> builder)
        {
            #region Attributes
            //Table
            builder.ToTable("RepositoryFile");

            // PK
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Id)
             .ValueGeneratedOnAdd();

            // Properties
            builder.Property(rf => rf.FilePath)
                .IsRequired();

            builder.Property(rf => rf.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(rf => rf.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(rf => rf.RepositoryId)
               .IsRequired();
            #endregion

            #region Relations
            builder.HasOne(rf => rf.Repository)
                    .WithMany(r => r.RepositoryFiles)
                    .HasForeignKey(rf => rf.RepositoryId)
                    .OnDelete(DeleteBehavior.Cascade); 
            #endregion

        }
    }
}
