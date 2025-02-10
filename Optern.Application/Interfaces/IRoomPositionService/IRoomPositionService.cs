

namespace Optern.Application.Interfaces.IRoomTrackService
{
    public interface IRoomPositionService
    {
        public Task<Response<IEnumerable<CreateRoomDTO>>> GetPositionRooms(int positionId);
        public Task<Response<bool>> AddRoomPosition(string roomID, IEnumerable<int> data);
        public Task<Response<bool>> DeleteRoomPosition(string roomID, int PositionId);

    }
}
