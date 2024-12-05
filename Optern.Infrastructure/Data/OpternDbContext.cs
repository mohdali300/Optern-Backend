using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Optern.Domain.Entities;
using System.Reflection.Emit;
using System;
using Task = Optern.Domain.Entities.Task;

namespace Optern.Infrastructure.Data
{
    public class OpternDbContext : IdentityDbContext<ApplicationUser>
    {
         public OpternDbContext(DbContextOptions<OpternDbContext> options) : base(options)
		 {
		 	
		 }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.ApplyConfigurationsFromAssembly(typeof(OpternDbContext).Assembly);
            base.OnModelCreating(builder);
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Chat> Chat { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CV> CV { get; set; }
        public DbSet<FavoritePosts> FavoritePosts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<PostTags> PostTags { get; set; }
        public DbSet<PTPFeedBack> PTPFeedBacks { get; set; }
        public DbSet<PTPInterview> PTPInterview { get; set; }
        public DbSet<PTPQuestions> PTPQuestions { get; set; }
        public DbSet<PTPUsers> PTPUsers{ get; set; }
        public DbSet<Reacts> Reacts { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<Sprint> Sprint { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }
        public DbSet<UserSkills> UserSkills{ get; set; }
        public DbSet<UserTasks> UserTasks{ get; set; }
        public DbSet<VFeedBack> VFeedBack { get; set; }
        public DbSet<VInterview> VInterview { get; set; }
        public DbSet<VQuestions> VQuestion { get; set; }
        public DbSet<WorkSpace> WorkSpace  { get; set; }
        public DbSet<Experience> Experiences  { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<SubTrack> SubTracks { get; set; }
        public DbSet<RoomTrack> RoomTracks { get; set; }

	}
}