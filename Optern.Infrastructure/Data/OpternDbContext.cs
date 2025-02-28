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
       
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatParticipants> ChatParticipants { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<CommentReacts> CommentReacts { get; set; }
        public DbSet<CV> CV { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<FavoritePosts> FavoritePosts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<RepositoryFile> RepositoryFiles { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostTags> PostTags { get; set; }
        public DbSet<PTPFeedBack> PTPFeedBacks { get; set; }
        public DbSet<PTPInterview> PTPInterviews { get; set; }

        public DbSet<PTPQuestionInterview> PTPQuestionInterviews { get; set; }
        public DbSet<PTPQuestions> PTPQuestions { get; set; }
        public DbSet<PTPUsers> PTPUsers { get; set; }
        public DbSet<Reacts> Reacts { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomSkills> RoomSkills { get; set; }
        public DbSet<RoomPosition> RoomPositions { get; set; }
        public DbSet<RoomTrack> RoomTracks { get; set; }

        public DbSet<Skills> Skills { get; set; }
        public DbSet<Sprint> Sprints { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<Tags> Tags { get; set; }
        public DbSet<Task> Tasks { get; set; }

        public DbSet<TaskActivity> TaskActivities { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }
        public DbSet<UserSkills> UserSkills { get; set; }
        public DbSet<UserTasks> UserTasks{ get; set; }
        public DbSet<VFeedBack> VFeedBack { get; set; }
        public DbSet<VInterview> VInterview { get; set; }

        public DbSet<VInterviewQuestions> VInterviewQuestions { get; set; }

        public DbSet<VQuestions> VQuestions { get; set; }
        public DbSet<WorkSpace> WorkSpaces  { get; set; }
        public DbSet<BookMarkedTask> BookMarkedTasks { get; set; }


    }
}