using Optern.Application.DTOs.Room;
using Optern.Application.Interfaces.IRoomService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Rooms.Query
{
    [ExtendObjectType("Query")]
    public class RoomQuery
    {
        [GraphQLDescription("GetAllRooms")]
        public async Task<Response<IEnumerable<RoomDTO>>> GetAllRooms([Service] IRoomService _roomService) =>
                   await _roomService.GetAllAsync();


        [GraphQLDescription("GetCreatedRooms")]
        public async Task<Response<IEnumerable<RoomDTO>>> GetCreatedRooms([Service] IRoomService _roomService, string id) =>
                  await _roomService.GetCreatedRooms(id);

        [GraphQLDescription("GetPopularRooms")]
        public async Task<Response<IEnumerable<RoomDTO>>> GetPopularRooms([Service] IRoomService _roomService) =>
                  await _roomService.GetPopularRooms();

        [GraphQLDescription("GetJoinedRooms")]
        public async Task<Response<IEnumerable<RoomDTO>>> GetJoinedRooms([Service] IRoomService _roomService, string id) =>
                  await _roomService.GetJoinedRooms(id);
    }
}
