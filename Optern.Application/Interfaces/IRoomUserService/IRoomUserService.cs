

namespace Optern.Application.Interfaces.IRoomUserService
{
    public interface IRoomUserService
    {

        public Task<Response<List<RoomUserDTO>>> GetAllCollaboratorsAsync(string roomId, bool? isAdmin = null);
        public Task<Response<List<RoomUserDTO>>> GetPendingRequestsAsync(string roomId, string leaderId);
        public Task<Response<string>> RequestToRoom(JoinRoomDTO model);
        public Task<Response<RoomUserDTO>> DeleteCollaboratorAsync(string RoomId, string TargetUserId, string leaderId);
        public Task<Response<RoomUserDTO>> ToggleLeadershipAsync(string roomId, string targetUserId, string leaderId);
        public Task<Response<List<RoomUserDTO>>> AcceptRequestsAsync(string roomId, string leaderId, int? userRoomId = null, bool? approveAll = null);
        public Task<Response<string>> RejectRequestsAsync(string roomId, string leaderId, int? userRoomId = null, bool? rejectAll = null);
    }
}
