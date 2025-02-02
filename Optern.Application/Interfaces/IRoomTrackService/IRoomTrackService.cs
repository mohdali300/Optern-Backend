using Optern.Infrastructure.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Application.Interfaces.IRoomTrackService
{
    public interface IRoomTrackService
    {
        public Task<Response<bool>> AddRoomTrack(string roomID, IEnumerable<int> data);
        public Task<Response<bool>> DeleteRoomTrack(string roomID, int trackId);
    }
}
