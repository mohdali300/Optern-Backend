﻿using Optern.Domain.Entities;
using Task = Optern.Domain.Entities.Task;

namespace Optern.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OpternDbContext _context;
        public IGenericRepository<PostTags> PostTags { get; private set; }
        public IGenericRepository<Room> Rooms { get; private set; }
        public IGenericRepository<UserRoom> UserRoom { get; private set; }
        public IGenericRepository<Track> Tracks { get; private set; }
        public IGenericRepository<Position> Positions { get; private set; }
        public IGenericRepository<ApplicationUser> Users { get; private set; }
        public IGenericRepository<RoomSkills> RoomSkills { get; private set; }
        public IGenericRepository<RoomPosition> RoomPositions { get; private set; }
        public IGenericRepository<FavoritePosts> FavoritePosts { get; private set; }
        public IGenericRepository<BookMarkedTask> BookMarkedTask { get; private set;  }
        public IGenericRepository<Skills>Skills { get; private set; }
        public IGenericRepository<RoomTrack> RoomTracks { get; private set; }
        public IGenericRepository<Post> Posts { get; private set; }

        public IGenericRepository<Tags> Tags { get; private set; }

        public IGenericRepository<Comment> Comments { get; private set; }

        public IGenericRepository<Reacts> Reacts { get; private set; }
        public IGenericRepository<WorkSpace> WorkSpace  { get; private set; }

        public IGenericRepository<Task> Tasks { get; private set; }

        public IGenericRepository<UserTasks> UserTasks { get; private set; }

        public IGenericRepository<TaskActivity> TaskActivites { get; private set; }

        public IGenericRepository<Sprint> Sprints { get; private set; }
        public IGenericRepository<UserRoom> RoomUsers { get; private set; }
        public IGenericRepository<Repository> Repository { get; private set; }
        public IGenericRepository<RepositoryFile> RepositoryFile { get; private set; }
        public IGenericRepository<Chat> Chats { get; private set; }
        public IGenericRepository<ChatParticipants> ChatParticipants { get; private set; }
        public IGenericRepository<Notifications> Notifications { get; private set; }
        public IGenericRepository<UserNotification> UserNotification { get; private set; }

        public IGenericRepository<Message> Messages { get; private set; }

        public IGenericRepository<PTPUsers> PTPUsers { get; private set; }
        public IGenericRepository<PTPFeedBack> PTPFeedBack { get; private set; }
        public IGenericRepository<PTPInterview> PTPInterviews { get; private set; }
        public IGenericRepository<PTPQuestionInterview> PTPQuestionInterviews { get; private set; }
        public IGenericRepository<VQuestionInterview> VQuestionInterview { get; private set; }
        public IGenericRepository<PTPQuestions> PTPQuestions { get; private set; }

        public IGenericRepository<Education> Education { get; private set; }
        public IGenericRepository<UserSkills> UserSkills { get; private set; }
        public IGenericRepository<Experience> Experience { get; private set; }
        public IGenericRepository<VInterview> VInterviews { get; private set; }

        public IGenericRepository<VFeedBack> VFeedBack {  get; private set; }


        public UnitOfWork(OpternDbContext context) 
        {
            _context = context;
            PostTags = new  GenericRepository<PostTags>(context);  
            Rooms = new  GenericRepository<Room>(context);
            UserRoom= new GenericRepository<UserRoom>(context);
            Tracks = new GenericRepository<Track>(context);
            Positions = new GenericRepository<Position>(context);
            Users= new GenericRepository<ApplicationUser>(context);
            RoomSkills= new GenericRepository<RoomSkills>(context);
            RoomPositions= new GenericRepository<RoomPosition>(context);
            FavoritePosts= new GenericRepository<FavoritePosts>(context);
            Posts= new GenericRepository<Post>(context);  
            Tags= new GenericRepository<Tags>(context);
            Comments= new GenericRepository<Comment>(context);
            Reacts= new GenericRepository<Reacts>(context);
            WorkSpace= new GenericRepository<WorkSpace>(context);
            Tasks= new GenericRepository<Task>(context);
            Sprints= new GenericRepository<Sprint>(context);
            RoomUsers= new GenericRepository<UserRoom>(context);
            BookMarkedTask=new GenericRepository<BookMarkedTask>(context);
            Skills= new GenericRepository<Skills>(context);
            UserTasks= new GenericRepository<UserTasks>(context);
            RoomTracks= new GenericRepository<RoomTrack>(context);
            TaskActivites = new GenericRepository<TaskActivity>(context);
            Repository= new GenericRepository<Repository>(context);
            RepositoryFile= new GenericRepository<RepositoryFile>(context);
            Chats= new GenericRepository<Chat>(context);
            ChatParticipants= new GenericRepository<ChatParticipants>(context);
            UserNotification= new GenericRepository<UserNotification>(context);
            Notifications= new GenericRepository<Notifications>(context);
            Messages= new GenericRepository<Message>(context);
            PTPUsers= new GenericRepository<PTPUsers>(context);
            PTPFeedBack =new GenericRepository<PTPFeedBack>(context);
            PTPInterviews=new GenericRepository<PTPInterview>(context);
            PTPQuestionInterviews= new GenericRepository<PTPQuestionInterview>(context);
            PTPQuestions= new GenericRepository<PTPQuestions>(context);
            Education= new GenericRepository<Education>(context);
            UserSkills= new GenericRepository<UserSkills>(context);
            Experience = new GenericRepository<Experience>(context);
            VInterviews= new GenericRepository<VInterview>(context);
            VFeedBack = new GenericRepository<VFeedBack>(context);
            VQuestionInterview = new GenericRepository<VQuestionInterview>(context);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
