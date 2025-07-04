﻿namespace Optern.Infrastructure.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            #region Attributes
            // Table Name

            builder.ToTable("Messages");

            // Primary Key
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
             .ValueGeneratedOnAdd();

            //Properties

            builder.Property(m => m.Content)
            .HasMaxLength(1000)
            .IsRequired();

            builder.Property(m => m.SentAt)
          .HasDefaultValueSql("NOW()")  
          .IsRequired();
            
            // Indexes
            builder.HasIndex(m => new { m.ChatId, m.SentAt, m.SenderId })
           .HasDatabaseName("IX_Messages_ChatId_SentDate_SenderId");


            #endregion

            #region Relations
            builder.HasOne(m => m.Sender)
                   .WithMany(u => u.Messages)
                   .HasForeignKey(m => m.SenderId) 
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Chat)
              .WithMany(c => c.Messages)
              .HasForeignKey(m => m.ChatId)
              .OnDelete(DeleteBehavior.Cascade);
            #endregion

        }
    }
}
