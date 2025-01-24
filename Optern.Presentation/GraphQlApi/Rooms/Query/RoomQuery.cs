using Optern.Application.DTOs.Room.RoomDTO;
using Optern.Application.Interfaces.IRoomService;
using Optern.Domain.Entities;
using Optern.Infrastructure.Response;

namespace Optern.Presentation.GraphQlApi.Rooms.Query
{
    public class RoomQuery
    {
        [GraphQLDescription("GetAll")]
        public async Task<Response<IEnumerable<RoomDTO>>> GetAllAsync([Service] IRoomService _roomService) =>
                      await _roomService.GetAllAsync();
    }
}
