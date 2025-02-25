using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
            var dictionaryComparer = new ValueComparer<Dictionary<int, string>>(
                        (d1, d2) => d1 != null && d2 != null && d1.SequenceEqual(d2),
                        d => d.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
                        d => d.ToDictionary(entry => entry.Key, entry => entry.Value) 
                    );

            builder.Property(f => f.Ratings)
               .HasColumnType("jsonb") 
               .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                        v => JsonSerializer.Deserialize<Dictionary<int, string>>(v, new JsonSerializerOptions()) ?? new Dictionary<int, string>()
                             )
                  .Metadata.SetValueComparer(dictionaryComparer);

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
