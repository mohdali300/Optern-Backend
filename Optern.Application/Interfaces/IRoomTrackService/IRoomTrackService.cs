

namespace Optern.Application.Interfaces.IRoomTrackService
{
    public interface IRoomTrackService
    {
        public Task<Response<bool>> AddRoomTrack(string roomID, IEnumerable<int> data);
        public Task<Response<bool>> DeleteRoomTrack(string roomID, int trackId);
    }
}
