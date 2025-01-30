using Optern.Application.DTOs.RoomUset;
using Optern.Application.Interfaces.IRoomUserService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.RoomUser.Mutation
{
    [ExtendObjectType("Mutation")]
    public class RoomUserMutation
    {
        [GraphQLDescription("Delete Room's Collaborator")]
        public async Task<Response<RoomUserDTO>> DeleteCollaboratorAsync([Service] IRoomUserService _roomUserService, string roomId, string targetUserId, string currentUserId)
            => await _roomUserService.DeleteCollaboratorAsync(roomId, targetUserId, currentUserId);

        [GraphQLDescription("Toggle user's leadership")]
        public async Task<Response<RoomUserDTO>> ToggleLeadershipAsync([Service] IRoomUserService _roomUserService,string roomId,string targetUserId,string currentUserId)
            => await _roomUserService.ToggleLeadershipAsync(roomId, targetUserId, currentUserId);
        


    }
}
