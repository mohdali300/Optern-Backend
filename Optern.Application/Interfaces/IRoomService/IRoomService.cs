
namespace Optern.Application.Interfaces.IRoomService
{
    public interface IRoomService
    {
        public Task<bool> IsRoomExist(string roomId);
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetAllAsync();
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetCreatedRooms(string id);
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetPopularRooms();
        public Task<Response<IEnumerable<ResponseRoomDTO>>> GetJoinedRooms(string id);
        public Task<Response<string>> JoinToRoom(JoinRoomDTO model);
        

        public Task<Response<ResponseRoomDTO>> CreateRoom(CreateRoomDTO model );
        public Task<Response<ResponseRoomDTO>> GetRoomById(string id,string? userId);
    }
}
