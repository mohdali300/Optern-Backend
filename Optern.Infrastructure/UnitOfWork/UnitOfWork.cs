using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;

namespace Optern.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OpternDbContext _context;
        public IGenericRepository<PostTags> PostTags { get; private set; }
        public IGenericRepository<Room> Rooms { get; private set; }
        public IGenericRepository<UserRoom> UserRoom { get; private set; }
        public IGenericRepository<Track> Tracks { get; private set; }
        public IGenericRepository<SubTrack> SubTracks { get; private set; }
        public IGenericRepository<ApplicationUser> Users { get; private set; }
        public IGenericRepository<RoomSkills> RoomSkills { get; private set; }
        public IGenericRepository<RoomTrack> RoomTracks { get; private set; }
        public IGenericRepository<FavoritePosts> FavoritePosts { get; private set; }
        public IGenericRepository<BookMarkedTask> BookMarkedTask { get; }


        public IGenericRepository<Post> Posts { get; private set; }

        public IGenericRepository<Tags> Tags { get; private set; }

        public IGenericRepository<Comment> Comments { get; private set; }

        public IGenericRepository<Reacts> Reacts { get; private set; }
        public IGenericRepository<WorkSpace> WorkSpace  { get; private set; }

        public UnitOfWork(OpternDbContext context) 
        {
            _context = context;
            PostTags = new  GenericRepository<PostTags>(context);  
            Rooms = new  GenericRepository<Room>(context);
            UserRoom= new GenericRepository<UserRoom>(context);
            Tracks = new GenericRepository<Track>(context);
            SubTracks = new GenericRepository<SubTrack>(context);
            Users= new GenericRepository<ApplicationUser>(context);
            RoomSkills= new GenericRepository<RoomSkills>(context);
            RoomTracks= new GenericRepository<RoomTrack>(context);
            FavoritePosts= new GenericRepository<FavoritePosts>(context);
            Posts= new GenericRepository<Post>(context);  
            Tags= new GenericRepository<Tags>(context);
            Comments= new GenericRepository<Comment>(context);
            Reacts= new GenericRepository<Reacts>(context);
            WorkSpace= new GenericRepository<WorkSpace>(context);
            BookMarkedTask=new GenericRepository<BookMarkedTask>(context);
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
