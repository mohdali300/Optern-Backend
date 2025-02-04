
    [ExtendObjectType("Query")]
    public class RoomQuery
    {
        [GraphQLDescription("GetAllRooms")]
        public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetAllRooms([Service] IRoomService _roomService) =>
                   await _roomService.GetAllAsync();


        [GraphQLDescription("GetCreatedRooms")]
        public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetCreatedRooms([Service] IRoomService _roomService, string id) =>
                  await _roomService.GetCreatedRooms(id);

        [GraphQLDescription("GetPopularRooms")]
        public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetPopularRooms([Service] IRoomService _roomService) =>
                  await _roomService.GetPopularRooms();

        [GraphQLDescription("GetJoinedRooms")]
        public async Task<Response<IEnumerable<ResponseRoomDTO>>> GetJoinedRooms([Service] IRoomService _roomService, string id) =>
                  await _roomService.GetJoinedRooms(id);

        [GraphQLDescription("Get Room By Id")]
        public async Task<Response<ResponseRoomDTO>> GetRoomByID([Service] IRoomService _roomService, string id,string? userId) =>
          await _roomService.GetRoomById(id,userId);
    }

