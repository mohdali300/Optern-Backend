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

            var listComparer = new ValueComparer<List<KeyValuePair<int, string>>>(
             (l1, l2) => (l1 == null && l2 == null) || (l1 != null && l2 != null && l1.SequenceEqual(l2)),
             l => l == null ? 0 : l.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
             l => l == null ? new List<KeyValuePair<int, string>>() : new List<KeyValuePair<int, string>>(l)
              );

            builder.Property(f => f.Ratings)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<KeyValuePair<int, string>>(), new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<KeyValuePair<int, string>>>(v, new JsonSerializerOptions()) ?? new List<KeyValuePair<int, string>>()
                )
                .Metadata.SetValueComparer(listComparer);

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
