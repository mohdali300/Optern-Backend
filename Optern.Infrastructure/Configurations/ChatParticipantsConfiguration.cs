namespace Optern.Infrastructure.Configurations
{
    public class ChatParticipantsConfiguration : IEntityTypeConfiguration<ChatParticipants>
    {
        public void Configure(EntityTypeBuilder<ChatParticipants> builder)
        {

            #region Attributes
            // Table Name

            builder.ToTable("ChatParticipants");

            // Primary Key 
            builder.HasKey(cp => cp.Id);
            builder.Property(cp => cp.Id)
            .ValueGeneratedOnAdd();

            //Properties

            builder.Property(cp => cp.JoinedAt)
               .IsRequired()
               .HasDefaultValueSql("NOW()");

            builder.Property(cp=>cp.UserId)
                .IsRequired();

            builder.Property(cp => cp.ChatId)
               .IsRequired();

            // Indexes
            builder.HasIndex(cp => new { cp.ChatId, cp.UserId })
               .IsUnique()
               .HasDatabaseName("IX_ChatParticipants_ChatId_UserId"); 


            #endregion

            #region Relations

            builder.HasOne(cp => cp.Chat)
                   .WithMany(c => c.ChatParticipants)
                   .HasForeignKey(cp => cp.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.User)
                   .WithMany(u => u.JoinedChats)
                   .HasForeignKey(cp => cp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            #endregion


        }
    }
}