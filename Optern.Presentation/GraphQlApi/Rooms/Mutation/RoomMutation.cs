

[ExtendObjectType("Mutation")]
    public class RoomMutation
    {

        [GraphQLDescription("Join To Room")]
        public async Task<Response<string>> JoinRoom([Service]IRoomService _roomService,JoinRoomDTO model)=>
            await _roomService.JoinToRoom(model);

        [GraphQLDescription("Create Room")]
        public async Task<Response<ResponseRoomDTO>> CreateRoom([Service] IRoomService _roomService, CreateRoomDTO model ,[GraphQLType(typeof(UploadType))] IFile? CoverPicture) =>
          await _roomService.CreateRoom(model, CoverPicture);    
    
    [GraphQLDescription("Edit Room")]
        public async Task<Response<string>> EditRoom([Service] IRoomSettingService _roomSetting, string id, EditRoomDTO? model ,IFile? CoverPicture) =>
          await _roomSetting.EditRoom(id,model!, CoverPicture); 
    
    [GraphQLDescription("Delete Room")]
        public async Task<Response<bool>> DeleteRoom([Service] IRoomSettingService _roomSetting, string id) =>
          await _roomSetting.DeleteRoom(id);  
    
    [GraphQLDescription("Reset Room")]
        public async Task<Response<bool>> ResetRoom([Service] IRoomSettingService _roomSetting, string id) =>
          await _roomSetting.ResetRoom(id);


}

