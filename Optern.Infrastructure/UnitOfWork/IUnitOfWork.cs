using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Domain.Entities;
using Optern.Infrastructure.Repositories;
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
        IGenericRepository<SubTrack> SubTracks { get; }
        IGenericRepository<ApplicationUser> Users { get; }
        IGenericRepository<RoomSkillsDTO> RoomSkills { get; }
        IGenericRepository<RoomTrack> RoomTracks { get; }
        IGenericRepository<Comment> Comments { get; }
        IGenericRepository<Reacts> Reacts { get; }
        IGenericRepository<WorkSpace> WorkSpace { get; }
        IGenericRepository<BookMarkedTask> BookMarkedTask { get; }
        IGenericRepository<Skills> Skills { get; }

            IGenericRepository<Task> Tasks { get; }

        IGenericRepository<Sprint> Sprints { get; }
        IGenericRepository<UserRoom> RoomUsers { get; }
        Task<int> SaveAsync();
    }
}
