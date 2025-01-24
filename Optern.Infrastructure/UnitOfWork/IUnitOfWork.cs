using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optern.Domain.Entities;
using Optern.Infrastructure.Repositories;

namespace Optern.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<PostTags> PostTags { get; }
        IGenericRepository<Room>Rooms { get; }
        IGenericRepository<Track> Tracks { get; }
        IGenericRepository<SubTrack> SubTracks { get; }

        Task<int> SaveAsync();
    }
}
