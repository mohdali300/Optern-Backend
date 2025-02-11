namespace Optern.Infrastructure.Configurations
{
    internal class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {

            #region Attributes
            // Table Name
            builder.ToTable("Chats");

            // Primary Key 
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

            //Properties

            builder.Property(c => c.CreatedDate)
                   .IsRequired()
                   .HasDefaultValueSql("NOW()");

            builder.Property(c => c.CreatorId)
                .IsRequired();

            builder.Property(c => c.Type)
                   .IsRequired()
                   .HasConversion<string>();
          
            #endregion

            #region Relations

            // one user create many Chats 
            builder.HasOne(c => c.Creator)
               .WithMany(u => u.CreatedChats)
               .HasForeignKey(c => c.CreatorId)
               .OnDelete(DeleteBehavior.NoAction);
            // m users can joined to m chats
            builder.HasMany(c => c.ChatParticipants)
                 .WithOne(cp => cp.Chat)
                 .HasForeignKey(cp => cp.ChatId) 
                 .OnDelete(DeleteBehavior.Cascade);
            // 1 to m , each chat has many Messages
            builder.HasMany(c => c.Messages)
                 .WithOne(m => m.Chat)
                 .HasForeignKey(m => m.ChatId)
                 .OnDelete(DeleteBehavior.Cascade);

            #endregion

        }
    }
}
