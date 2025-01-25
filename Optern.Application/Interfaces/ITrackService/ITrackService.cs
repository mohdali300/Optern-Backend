using Optern.Application.DTOs.Track;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.ITrackService
{
    public interface ITrackService
    {
        public Task<Response<List<TrackDTO>>> GetAll();
        public Task<Response<List<TrackWithSubTracksDTO>>> GetAllWithSubTracks();
        public Task<Response<TrackDTO>> Add(string name);
    }
}
