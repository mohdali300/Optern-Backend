using Optern.Application.DTOs.SubTrack;
using Optern.Application.DTOs.Track;
using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.ISubTrackService
{
    public interface ISubTrackService
    {
        public Task<Response<List<SubTrackDTO>>> GetAllByTrackId(int trackId);
        public Task<Response<SubTrackDTO>> Add(string name, int trackId);
    }
}
