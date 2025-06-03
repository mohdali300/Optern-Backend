
namespace Optern.Application.Interfaces.IRoomService
{
    public interface IRoomService
    {
        public Task<bool> IsRoomExist(string roomId);
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetAllAsync();
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetCreatedRooms(string id ,int lastIdx = 0, int limit = 10);
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetPopularRooms();
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetJoinedRooms(string id, int lastIdx = 0, int limit = 10);

        public Task<Response<string>> JoinToRoom(JoinRoomDTO model);
        public Task<Response<IEnumerable<ResponseRoomDTO>>> SearchRooms(string? roomName = null, int? trackId = null, int lastIdx = 0, int limit = 10);

        public Task<Response<ResponseRoomDTO>> CreateRoom(CreateRoomDTO model );
        public Task<Response<ResponseRoomDTO>> GetRoomById(string id,string? userId);
    }
}
