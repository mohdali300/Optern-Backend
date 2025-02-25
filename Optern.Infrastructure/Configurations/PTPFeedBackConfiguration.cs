using static Dapper.SqlMapper;

namespace Optern.Infrastructure.Configurations
{
    public class PTPFeedBackConfiguration : IEntityTypeConfiguration<PTPFeedBack>
    {
        public void Configure(EntityTypeBuilder<PTPFeedBack> builder)
        {

            #region Attributes 

            // Table Name

            builder.ToTable("PTPFeedBacks");

            // Primary Key

            builder.HasKey(fb => fb.Id);
            builder.Property(fb => fb.Id)
                   .ValueGeneratedOnAdd();

             // Properties
        

              builder.Property(fb => fb.PTPInterviewId)
                   .IsRequired();

            // Indexes

            builder.HasIndex(fb => fb.PTPInterviewId)
              .HasDatabaseName("IX_PTPFeedBack_PTPInterviewId");

            builder.HasIndex(f => new { f.GivenByUserId, f.ReceivedByUserId, f.PTPInterviewId })
                  .IsUnique();

            #endregion

            #region Relations
            builder.HasOne(fb => fb.PTPInterview)
                   .WithMany(ptp => ptp.PTPFeedBacks)
                   .HasForeignKey(fb => fb.PTPInterviewId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.GivenByUser)
             .WithMany()
             .HasForeignKey(f => f.GivenByUserId)
             .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(f => f.ReceivedByUser)
                  .WithMany()
                  .HasForeignKey(f => f.ReceivedByUserId)
                  .OnDelete(DeleteBehavior.Cascade); 



            #endregion


        }
    }

}
