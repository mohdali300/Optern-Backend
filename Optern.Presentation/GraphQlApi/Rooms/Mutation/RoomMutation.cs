
    [ExtendObjectType("Mutation")]
    public class RoomMutation
    {

        [GraphQLDescription("Join To Room")]
        public async Task<Response<string>> JoinRoom([Service]IRoomService _roomService,JoinRoomDTO model)=>
            await _roomService.JoinToRoom(model);

        [GraphQLDescription("Create Room")]
        public async Task<Response<ResponseRoomDTO>> CreateRoom([Service] IRoomService _roomService, CreateRoomDTO model ,IFile CoverPicture) =>
          await _roomService.CreateRoom(model, CoverPicture);

    }

