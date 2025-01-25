using Optern.Application.DTOs.Room;
using Optern.Application.Interfaces.IRoomService;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Rooms.Mutation
{
    [ExtendObjectType("Mutation")]
    public class RoomMutation
    {

        [GraphQLDescription("Join To Room")]
        public async Task<Response<string>> JoinRoom([Service]IRoomService _roomService,JoinRoomDTO model)=>
            await _roomService.JoinToRoom(model);

    }
}
