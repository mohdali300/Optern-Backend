using Optern.Application.DTOs.Room;
using Optern.Application.DTOs.RoomUset;
using Optern.Application.Interfaces.IRoomService;
using Optern.Application.Interfaces.IRoomUserService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.RoomUser.Mutation
{
    [ExtendObjectType("Mutation")]
    public class RoomUserMutation
    {
        [GraphQLDescription("Request to Join To Room")]
        public async Task<Response<string>> RequestToRoom([Service] IRoomUserService _roomUserService, JoinRoomDTO model) 
            =>await _roomUserService.RequestToRoom(model);

        [GraphQLDescription("Delete Room's Collaborator")]
        public async Task<Response<RoomUserDTO>> DeleteCollaboratorAsync([Service] IRoomUserService _roomUserService, string roomId, string targetUserId, string currentUserId)
            => await _roomUserService.DeleteCollaboratorAsync(roomId, targetUserId, currentUserId);

        [GraphQLDescription("Toggle user's leadership")]
        public async Task<Response<RoomUserDTO>> ToggleLeadershipAsync([Service] IRoomUserService _roomUserService,string roomId,string targetUserId,string currentUserId)
            => await _roomUserService.ToggleLeadershipAsync(roomId, targetUserId, currentUserId);

        [GraphQLDescription("Accept Joined Requests")]
        public async Task<Response<List<RoomUserDTO>>> AcceptRequestsAsync([Service] IRoomUserService _roomUserService,string roomId, string leaderId, int? userRoomId = null, bool? approveAll = null)
            =>await _roomUserService.AcceptRequestsAsync(roomId, leaderId, userRoomId, approveAll);

        [GraphQLDescription("Reject Joined Requests")]
        public async Task<Response<List<RoomUserDTO>>> RejectRequestsAsync([Service] IRoomUserService _roomUserService,string roomId, string leaderId, int? userRoomId = null, bool? rejectAll = null)
            =>await _roomUserService.RejectRequestsAsync(roomId,leaderId, userRoomId, rejectAll);




    }
}
