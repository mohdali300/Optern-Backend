
    [ExtendObjectType("Query")]
    public class RoomQuery
    {
        [GraphQLDescription("GetAllRooms")]
        public async Task<Response<IEnumerable<CreateRoomDTO>>> GetAllRooms([Service] IRoomService _roomService) =>
                   await _roomService.GetAllAsync();


        [GraphQLDescription("GetCreatedRooms")]
        public async Task<Response<IEnumerable<CreateRoomDTO>>> GetCreatedRooms([Service] IRoomService _roomService, string id) =>
                  await _roomService.GetCreatedRooms(id);

        [GraphQLDescription("GetPopularRooms")]
        public async Task<Response<IEnumerable<CreateRoomDTO>>> GetPopularRooms([Service] IRoomService _roomService) =>
                  await _roomService.GetPopularRooms();

        [GraphQLDescription("GetJoinedRooms")]
        public async Task<Response<IEnumerable<CreateRoomDTO>>> GetJoinedRooms([Service] IRoomService _roomService, string id) =>
                  await _roomService.GetJoinedRooms(id);

        [GraphQLDescription("Get Room By Id")]
        public async Task<Response<RoomDetailsDTO>> GetRoomByID([Service] IRoomService _roomService, string id) =>
          await _roomService.GetRoomById(id);
    }

