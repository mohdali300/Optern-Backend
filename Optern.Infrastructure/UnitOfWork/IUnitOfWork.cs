using Task = Optern.Domain.Entities.Task;

namespace Optern.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<FavoritePosts> FavoritePosts { get; }
        IGenericRepository<PostTags> PostTags { get; }
        IGenericRepository<Tags> Tags { get; }

        IGenericRepository<Post> Posts {  get; }
        IGenericRepository<Room>Rooms { get; }
        IGenericRepository<UserRoom> UserRoom { get; }
        IGenericRepository<Track> Tracks { get; }
        IGenericRepository<Position> Positions { get; }
        IGenericRepository<ApplicationUser> Users { get; }
        IGenericRepository<RoomSkills> RoomSkills { get; }
        IGenericRepository<RoomPosition> RoomPositions { get; }
        IGenericRepository<Comment> Comments { get; }
        IGenericRepository<Reacts> Reacts { get; }
        IGenericRepository<WorkSpace> WorkSpace { get; }
        IGenericRepository<BookMarkedTask> BookMarkedTask { get; }
        IGenericRepository<Skills> Skills { get; }
        IGenericRepository<RoomTrack> RoomTracks { get; }
        IGenericRepository<Task> Tasks { get; }

        IGenericRepository<UserTasks> UserTasks { get; }

        IGenericRepository<TaskActivity> TaskActivites { get; }

        IGenericRepository<Sprint> Sprints { get; }
        IGenericRepository<UserRoom> RoomUsers { get; }
        IGenericRepository<Repository> Repository { get; }
        IGenericRepository<RepositoryFile> RepositoryFile { get;  }
        IGenericRepository<Chat> Chats { get; }

        IGenericRepository<Notifications> Notifications { get; }
        IGenericRepository<UserNotification> UserNotification { get; }

        IGenericRepository<ChatParticipants> ChatParticipants { get; }
        IGenericRepository<Message> Messages { get; }

        IGenericRepository<PTPUsers> PTPUsers { get; }

        IGenericRepository<PTPFeedBack> PTPFeedBack { get; }

        IGenericRepository<PTPInterview> PTPInterviews { get; }  

        IGenericRepository<PTPQuestionInterview> PTPQuestionInterviews { get; } 

        IGenericRepository<PTPQuestions> PTPQuestions { get; }

        Task<int> SaveAsync();
    }
}
