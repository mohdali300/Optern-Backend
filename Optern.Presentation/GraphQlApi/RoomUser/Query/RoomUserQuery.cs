using Optern.Application.DTOs.RoomUset;
using Optern.Application.Interfaces.IRoomUserService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.RoomUser.Query
{
    [ExtendObjectType("Query")]
    public class RoomUserQuery
    {
        [GraphQLDescription("All Room's Collaborators")]
        public async Task<Response<List<RoomUserDTO>>> GetAllCollaboratorsAsync([Service] IRoomUserService _roomUserService, string roomId, bool? isAdmin = null)
            => await _roomUserService.GetAllCollaboratorsAsync(roomId, isAdmin);
    }
}
