﻿using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Optern.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            #region Attributes

            // Table Name

            builder.ToTable("ApplicationUser");

            // Properties


            builder.Property(u => u.FirstName)
             .IsRequired()
             .HasMaxLength(50);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>() 
                .HasMaxLength(50);      


            builder.Property(u => u.JobTitle)
                .IsRequired(false)
                .HasMaxLength(50);

            builder.Property(u => u.ProfilePicture)
                .IsRequired(false).HasMaxLength(250);

            builder.Property(u => u.Email)
                .IsRequired();
            
            builder.Property(u => u.Gender)
                .IsRequired(false).HasMaxLength(20); ;  
            
            builder.Property(u => u.Country)
                .IsRequired(false).HasMaxLength(100);

            builder.Property(u => u.CreatedAt)
                .IsRequired() 
                .HasDefaultValueSql("NOW()");

            builder.Property(u => u.LastLogIn)
                .IsRequired(false);

            builder.Property(u => u.Links)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions()) ?? new Dictionary<string, string>()
                )
                .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())), 
                    c => new Dictionary<string, string>(c) 
                ));



            // Indexes

            builder.HasIndex(u => u.Email)
             .HasDatabaseName("IX_ApplicationUsers_Email");


            #endregion

            #region Relations

            builder.HasOne(u => u.Position)
                 .WithMany(p => p.Users)
                 .HasForeignKey(u => u.PositionId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.Rooms)
                .WithOne(r => r.Creator)
                .HasForeignKey(u => u.CreatorId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.NoAction);

            // M TO M : delete user joined rooms if user deleted 
            builder.HasMany(u => u.UserRooms)
               .WithOne(ur => ur.User)
               .HasForeignKey(ur => ur.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            // M TO M : delete user joined chats if user deleted 
            builder.HasMany(u => u.JoinedChats)
              .WithOne(cp => cp.User) 
              .HasForeignKey(cp => cp.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            // if user is deleted don't delete Chat
            builder.HasMany(u => u.CreatedChats)
               .WithOne(c => c.Creator) 
               .HasForeignKey(c => c.CreatorId) 
               .OnDelete(DeleteBehavior.NoAction);

            // if Sender is deleted don't delete his Messages
            builder.HasMany(u => u.Messages)
              .WithOne(m => m.Sender) 
              .HasForeignKey(m => m.SenderId) 
              .OnDelete(DeleteBehavior.NoAction);

            // cascade remove any record for joinedInterviews if user is deleted 
            builder.HasMany(u => u.JoinedVirtualInterviews)
              .WithOne(vi => vi.User) 
              .HasForeignKey(vi => vi.UserId) 
              .OnDelete(DeleteBehavior.Cascade);

            // cascade remove any record for createdPosts if user is deleted 
            builder.HasMany(u => u.CreatedPosts)
               .WithOne(p => p.Creator) 
               .HasForeignKey(p => p.CreatorId) 
               .OnDelete(DeleteBehavior.Cascade);

            // remove user reacts if user deleted
            builder.HasMany(u => u.Reacts)
               .WithOne(r => r.User) 
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            // remove user comments if user deleted
            builder.HasMany(u => u.Comments)
               .WithOne(c => c.User) 
               .HasForeignKey(c => c.UserId) 
               .OnDelete(DeleteBehavior.Cascade);

            // remove user comments if user deleted
            builder.HasMany(u => u.FavoritePosts)
              .WithOne(fp => fp.User) 
              .HasForeignKey(fp => fp.UserId) 
              .OnDelete(DeleteBehavior.Cascade);
            // m to m relation , remove userTasks if user is deleted 
            builder.HasMany(u => u.AssignedTasks)
              .WithOne(ut => ut.User) 
              .HasForeignKey(ut => ut.UserId) 
              .OnDelete(DeleteBehavior.Cascade);

            // here cuz in this type of interview has Two Persons don't force delete PTP for another User
            builder.HasMany(u => u.PeerToPeerInterviewUsers)
           .WithOne(ptp => ptp.User) 
           .HasForeignKey(ptp => ptp.UserID) 
           .OnDelete(DeleteBehavior.NoAction);

            // if user deleted remove any related Skills
            builder.HasMany(u => u.UserSkills)
              .WithOne(us => us.User) 
              .HasForeignKey(us => us.UserId) 
              .OnDelete(DeleteBehavior.Cascade);

            // 1 to m delete user Cvs if user deleted
            builder.HasMany(u => u.CVs) 
             .WithOne(cv => cv.User) 
             .HasForeignKey(cv => cv.UserId) 
             .OnDelete(DeleteBehavior.Cascade);
            // m to m , delete all notifications if user deleted
            builder.HasMany(u => u.UserNotification)
              .WithOne(un => un.User) 
              .HasForeignKey(un => un.UserId) 
              .OnDelete(DeleteBehavior.Cascade);
            // cascade delete
            builder.HasMany(u => u.CommentReacts)
             .WithOne(cr => cr.User)    
             .HasForeignKey(cr => cr.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Experiences)
             .WithOne(e => e.User) 
             .HasForeignKey(e => e.UserId) 
             .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Educations)
              .WithOne(e => e.User) 
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.BookMarkedTasks)
              .WithOne(e => e.User)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);
            #endregion

        }
    }
}
